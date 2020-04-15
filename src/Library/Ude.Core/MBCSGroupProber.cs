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
using System.Collections.Generic;

namespace Ude.Core
{
    /// <summary>
    /// Multi-byte charsets probers
    /// </summary>
    public class MBCSGroupProber : CharsetProber
    {
        private static int PROBERS_NUM; // = 7;
        private Dictionary<CharsetProber, string> probersByName;
        private CharsetProber[] probers; // = new CharsetProber[PROBERS_NUM];
        private bool[] isActive; // = new bool[PROBERS_NUM];
        private int bestGuess;
        private int activeNum;
            
        public MBCSGroupProber()
        {
	        this.probersByName = new Dictionary<CharsetProber, string>();
	        this.probersByName.Add(new UTF8Prober(), "UTF8");
	        //this.probersByName.Add(new SJISProber(), "SJIS");
	        //this.probersByName.Add(new EUCJPProber(), "EUCJP");
	        //this.probersByName.Add(new GB18030Prober(), "GB18030");
	        //this.probersByName.Add(new EUCKRProber(), "EUCKR");
	        //this.probersByName.Add(new Big5Prober(), "Big5");
	        //this.probersByName.Add(new EUCTWProber(), "EUCTW");

	        PROBERS_NUM = this.probersByName.Count;
			this.isActive = new bool[this.probersByName.Count];
			this.probers = new CharsetProber[this.probersByName.Count];
	        int i = 0;
	        foreach (KeyValuePair<CharsetProber, string> pair in this.probersByName)
	        {
		        this.probers[i++] = pair.Key;
	        }

	        this.Reset();        
        }

        public override string GetCharsetName()
        {
            if (this.bestGuess == -1) {
	            this.GetConfidence();
                if (this.bestGuess == -1) this.bestGuess = 0;
            }
            return this.probers[this.bestGuess].GetCharsetName();
        }

        public override void Reset()
        {
	        this.activeNum = 0;
            for (int i = 0; i < this.probers.Length; i++) {
                if (this.probers[i] != null) {
	                this.probers[i].Reset();
	                this.isActive[i] = true;
                   ++this.activeNum;
                } else {
	                this.isActive[i] = false;
                }
            }
	        this.bestGuess = -1;
	        this.state = ProbingState.Detecting;
        }

        public override ProbingState HandleData(byte[] buf, int offset, int len)
        {
            // do filtering to reduce load to probers
            byte[] highbyteBuf = new byte[len];
            int hptr = 0;
            //assume previous is not ascii, it will do no harm except add some noise
            bool keepNext = true;
            int max = offset + len;
            
            for (int i = offset; i < max; i++) {
                if ((buf[i] & 0x80) != 0) {
                    highbyteBuf[hptr++] = buf[i];
                    keepNext = true;
                } else {
                    //if previous is highbyte, keep this even it is a ASCII
                    if (keepNext) {
                        highbyteBuf[hptr++] = buf[i];
                        keepNext = false;
                    }
                }
            }
            
            ProbingState st = ProbingState.NotMe;
            
            for (int i = 0; i < this.probers.Length; i++) {
                if (!this.isActive[i])
                    continue;
                st = this.probers[i].HandleData(highbyteBuf, 0, hptr);
                if (st == ProbingState.FoundIt) {
	                this.bestGuess = i;
	                this.state = ProbingState.FoundIt;
                    break;
                } else if (st == ProbingState.NotMe) {
	                this.isActive[i] = false;
	                this.activeNum--;
                    if (this.activeNum <= 0) {
	                    this.state = ProbingState.NotMe;
                        break;
                    }
                }
            }
            return this.state;
        }

        public override float GetConfidence()
        {
            float bestConf = 0.0f;
            float cf = 0.0f;
            
            if (this.state == ProbingState.FoundIt) {
                return 0.99f;
            } else if (this.state == ProbingState.NotMe) {
                return 0.01f;
            } else {
                for (int i = 0; i < PROBERS_NUM; i++) {
                    if (!this.isActive[i])
                        continue;
                    cf = this.probers[i].GetConfidence();
                    if (bestConf < cf) {
                        bestConf = cf;
	                    this.bestGuess = i;
                    }
                }
            }
            return bestConf;
        }

        public override void DumpStatus()
        {
            float cf;
	        this.GetConfidence();
            for (int i = 0; i < PROBERS_NUM; i++) {
                if (!this.isActive[i]) {
                  
                } else {
                    cf = this.probers[i].GetConfidence();
                }
            }
        }
    }
}
