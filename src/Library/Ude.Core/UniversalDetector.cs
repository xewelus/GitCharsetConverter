/* ***** BEGIN LICENSE BLOCK *****
 * Version: MPL 1.1/GPL 2.0/LGPL 2.1
 *
 * The contents of this file are subject to the Mozilla Public License Version
 * 1.1 (the "License"); you may not use this file except in compliance with
 * the License. You may obtain a copy of the License at
 * http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the
 * License.
 *
 * The Original Code is Mozilla Universal charset detector code.
 *
 * The Initial Developer of the Original Code is
 * Netscape Communications Corporation.
 * Portions created by the Initial Developer are Copyright (C) 2001
 * the Initial Developer. All Rights Reserved.
 *
 * Contributor(s):
 *          Shy Shalom <shooshX@gmail.com>
 *          Rudi Pettazzi <rudi.pettazzi@gmail.com> (C# port)
 * 
 * Alternatively, the contents of this file may be used under the terms of
 * either the GNU General Public License Version 2 or later (the "GPL"), or
 * the GNU Lesser General Public License Version 2.1 or later (the "LGPL"),
 * in which case the provisions of the GPL or the LGPL are applicable instead
 * of those above. If you wish to allow use of your version of this file only
 * under the terms of either the GPL or the LGPL, and not to allow others to
 * use your version of this file under the terms of the MPL, indicate your
 * decision by deleting the provisions above and replace them with the notice
 * and other provisions required by the GPL or the LGPL. If you do not delete
 * the provisions above, a recipient may use your version of this file under
 * the terms of any one of the MPL, the GPL or the LGPL.
 *
 * ***** END LICENSE BLOCK ***** */

using System;

namespace Ude.Core
{

    enum InputState { PureASCII=0, EscASCII=1, Highbyte=2 };

    public abstract class UniversalDetector 
    {
        protected const int FILTER_CHINESE_SIMPLIFIED = 1;
        protected const int FILTER_CHINESE_TRADITIONAL = 2;
        protected const int FILTER_JAPANESE = 4;
        protected const int FILTER_KOREAN = 8;
        protected const int FILTER_NON_CJK = 16;
        protected const int FILTER_ALL = 31;
        protected static int FILTER_CHINESE = 
            FILTER_CHINESE_SIMPLIFIED | FILTER_CHINESE_TRADITIONAL;
        protected static int FILTER_CJK = 
                FILTER_JAPANESE | FILTER_KOREAN | FILTER_CHINESE_SIMPLIFIED 
                | FILTER_CHINESE_TRADITIONAL;
        
        protected const float SHORTCUT_THRESHOLD = 0.95f;
        protected const float MINIMUM_THRESHOLD = 0.20f;

        internal InputState inputState;
        protected bool start;
        protected bool gotData;
        protected bool done;
        protected byte lastChar;
        protected int bestGuess;
        protected const int PROBERS_NUM = 3;
        protected int languageFilter;
        protected CharsetProber[] charsetProbers = new CharsetProber[PROBERS_NUM];
        protected CharsetProber escCharsetProber;
        protected string detectedCharset;

        public UniversalDetector(int languageFilter) { 
            this.start = true;
            this.inputState = InputState.PureASCII;
            this.lastChar = 0x00;   
            this.bestGuess = -1;
            this.languageFilter = languageFilter;
        }

        public virtual void Feed(byte[] buf, int offset, int len)
        { 
            if (this.done) {
                return;
            }

            if (len > 0) this.gotData = true;

            // If the data starts with BOM, we know it is UTF
            if (this.start) {
	            this.start = false;
                if (len > 3) {
                    switch (buf[0]) {
                    case 0xEF:
                        if (0xBB == buf[1] && 0xBF == buf[2]) this.detectedCharset = "UTF-8";
                        break;
                    case 0xFE:
                        if (0xFF == buf[1] && 0x00 == buf[2] && 0x00 == buf[3])
                            // FE FF 00 00  UCS-4, unusual octet order BOM (3412)
	                        this.detectedCharset = "X-ISO-10646-UCS-4-3412";
                        else if (0xFF == buf[1]) this.detectedCharset = "UTF-16BE";
                        break;
                    case 0x00:
                        if (0x00 == buf[1] && 0xFE == buf[2] && 0xFF == buf[3])
	                        this.detectedCharset = "UTF-32BE";
                        else if (0x00 == buf[1] && 0xFF == buf[2] && 0xFE == buf[3])
                            // 00 00 FF FE  UCS-4, unusual octet order BOM (2143)
	                        this.detectedCharset = "X-ISO-10646-UCS-4-2143";
                        break;
                    case 0xFF:
                        if (0xFE == buf[1] && 0x00 == buf[2] && 0x00 == buf[3])
	                        this.detectedCharset = "UTF-32LE";
                        else if (0xFE == buf[1]) this.detectedCharset = "UTF-16LE";
                        break;
                    }  // switch
                }
                if (this.detectedCharset != null) {
	                this.done = true;
                    return;
                }
            }

            for (int i = 0; i < len; i++) {
                
                // other than 0xa0, if every other character is ascii, the page is ascii
                if ((buf[i] & 0x80) != 0 && buf[i] != 0xA0)  {
                    // we got a non-ascii byte (high-byte)
                    if (this.inputState != InputState.Highbyte) {
	                    this.inputState = InputState.Highbyte;

                        // kill EscCharsetProber if it is active
                        if (this.escCharsetProber != null) {
	                        this.escCharsetProber = null;
                        }

                        // start multibyte and singlebyte charset prober
                        if (this.charsetProbers[0] == null) this.charsetProbers[0] = new MBCSGroupProber();
                        if (this.charsetProbers[1] == null) this.charsetProbers[1] = new SBCSGroupProber();
                        //if (charsetProbers[2] == null)
                        //    charsetProbers[2] = new Latin1Prober(); 
                    }
                } else { 
                    if (this.inputState == InputState.PureASCII &&
                        (buf[i] == 0x1B || (buf[i] == 0x7B && this.lastChar == 0x7E))) {
                        // found escape character or HZ "~{"
	                    this.inputState = InputState.EscASCII;
                    }
	                this.lastChar = buf[i];
                }
            }
            
            ProbingState st = ProbingState.NotMe;
            
            switch (this.inputState) {
                case InputState.EscASCII:
                    if (this.escCharsetProber == null) {
	                    this.escCharsetProber = new EscCharsetProber();
                    }
                    st = this.escCharsetProber.HandleData(buf, offset, len);
                    if (st == ProbingState.FoundIt) {
	                    this.done = true;
	                    this.detectedCharset = this.escCharsetProber.GetCharsetName();
                    }
                    break;
                case InputState.Highbyte:
                    for (int i = 0; i < PROBERS_NUM; i++) {
                        if (this.charsetProbers[i] != null) {
                            st = this.charsetProbers[i].HandleData(buf, offset, len);
                            #if DEBUG
	                        this.charsetProbers[i].DumpStatus();
                            #endif                        
                            if (st == ProbingState.FoundIt) {
	                            this.done = true;
	                            this.detectedCharset = this.charsetProbers[i].GetCharsetName();
                                return;
                            }  
                        }
                    }
                    break;
                default:  
                    // pure ascii
                    break;
            }
            return;
        }

        /// <summary>
        /// Notify detector that no further data is available. 
        /// </summary>
        public virtual void DataEnd()
        {
            if (!this.gotData) {
                // we haven't got any data yet, return immediately 
                // caller program sometimes call DataEnd before anything has 
                // been sent to detector
                return;
            }

            if (this.detectedCharset != null) {
	            this.done = true;
	            this.Report(this.detectedCharset, 1.0f);
                return;
            } 

            if (this.inputState == InputState.Highbyte) {
                float proberConfidence = 0.0f;
                float maxProberConfidence = 0.0f;
                int maxProber = 0;
                for (int i = 0; i < PROBERS_NUM; i++) {
                    if (this.charsetProbers[i] != null) {
                        proberConfidence = this.charsetProbers[i].GetConfidence();
                        if (proberConfidence > maxProberConfidence) {
                            maxProberConfidence = proberConfidence;
                            maxProber = i;
                        }
                    }
                }
                
                if (maxProberConfidence > MINIMUM_THRESHOLD) {
	                this.Report(this.charsetProbers[maxProber].GetCharsetName(), maxProberConfidence);
                } 
                
            } else if (this.inputState == InputState.PureASCII) {
	            this.Report("ASCII", 1.0f);
            } 
        }

        /// <summary>
        /// Clear internal state of charset detector.
        /// In the original interface this method is protected. 
        /// </summary>
        public virtual void Reset() 
        {
	        this.done = false;
	        this.start = true;
	        this.detectedCharset = null;
	        this.gotData = false;
	        this.bestGuess = -1;
	        this.inputState = InputState.PureASCII;
	        this.lastChar = 0x00;
            if (this.escCharsetProber != null) this.escCharsetProber.Reset();
            for (int i = 0; i < PROBERS_NUM; i++)
                if (this.charsetProbers[i] != null)
		                this.charsetProbers[i].Reset();
        }
        
        protected abstract void Report(string charset, float confidence);

    }
}
