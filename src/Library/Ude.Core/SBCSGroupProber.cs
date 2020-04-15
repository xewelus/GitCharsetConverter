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
    public class SBCSGroupProber : CharsetProber
    {
        private static int PROBERS_NUM;//; = 13;
        private CharsetProber[] probers;// = new CharsetProber[PROBERS_NUM];        
        private bool[] isActive;// = new bool[PROBERS_NUM];
        private int bestGuess;
        private int activeNum;

	    public SBCSGroupProber()
	    {
		    List<CharsetProber> list = new List<CharsetProber>();
		    list.Add(new SingleByteCharSetProber(new Win1251Model()));
		   // list.Add(new SingleByteCharSetProber(new Koi8rModel()));
		    //list.Add(new SingleByteCharSetProber(new Latin5Model()));
		    //list.Add(new SingleByteCharSetProber(new MacCyrillicModel()));
		    list.Add(new SingleByteCharSetProber(new Ibm866Model()));
		    //list.Add(new SingleByteCharSetProber(new Ibm855Model()));
		    //list.Add(new SingleByteCharSetProber(new Latin7Model()));
		    //list.Add(new SingleByteCharSetProber(new Win1253Model()));
		    //list.Add(new SingleByteCharSetProber(new Latin5BulgarianModel()));
		    //list.Add(new SingleByteCharSetProber(new Win1251BulgarianModel()));

		    //HebrewProber hebprober = new HebrewProber();
		    //list.Add(hebprober);
		    //// Logical  
		    //SingleByteCharSetProber hebprober1 = new SingleByteCharSetProber(new Win1255Model(), false, hebprober);

		    //list.Add(hebprober1);
		    //// Visual
		    //SingleByteCharSetProber hebprober2 = new SingleByteCharSetProber(new Win1255Model(), true, hebprober);
		    //list.Add(hebprober2);

		    //hebprober.SetModelProbers(hebprober1, hebprober2);

		    this.probers = list.ToArray();
		    PROBERS_NUM = this.probers.Length;
		    this.isActive = new bool[this.probers.Length];

		    this.Reset();
		}

		public override ProbingState HandleData(byte[] buf, int offset, int len) 
        {
            ProbingState st = ProbingState.NotMe;
            
            //apply filter to original buffer, and we got new buffer back
            //depend on what script it is, we will feed them the new buffer 
            //we got after applying proper filter
            //this is done without any consideration to KeepEnglishLetters
            //of each prober since as of now, there are no probers here which
            //recognize languages with English characters.
            byte[] newBuf = FilterWithoutEnglishLetters(buf, offset, len);
            if (newBuf.Length == 0)
                return this.state; // Nothing to see here, move on.
            
            for (int i = 0; i < PROBERS_NUM; i++) {
                if (!this.isActive[i])
                    continue;
                st = this.probers[i].HandleData(newBuf, 0, newBuf.Length);
                
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
            float bestConf = 0.0f, cf;
            switch (this.state) {
            case ProbingState.FoundIt:
                return 0.99f; //sure yes
            case ProbingState.NotMe:
                return 0.01f;  //sure no
            default:
                for (int i = 0; i < PROBERS_NUM; i++)
                {
                    if (!this.isActive[i])
                        continue;
                    cf = this.probers[i].GetConfidence();
                    if (bestConf < cf)
                    {
                        bestConf = cf;
	                    this.bestGuess = i;
                    }
                }
                break;
            }
            return bestConf;
        }

        public override void DumpStatus()
        {
            float cf = this.GetConfidence();
	        if (CharsetDetector.NeedConsoleLog)
	        {
		        Console.WriteLine(" SBCS Group Prober --------begin status");
	        }
	        for (int i = 0; i < PROBERS_NUM; i++) {
	            if (!this.isActive[i])
	            {
		            if (CharsetDetector.NeedConsoleLog)
		            {
			            Console.WriteLine(" inactive: [{0}] (i.e. confidence is too low).", this.probers[i].GetCharsetName());
		            }
	            }
	            else
	            {
		            this.probers[i].DumpStatus();
	            }
            }
	        if (CharsetDetector.NeedConsoleLog)
	        {
		        Console.WriteLine(" SBCS Group found best match [{0}] confidence {1}.", this.probers[this.bestGuess].GetCharsetName(), cf);
	        }
        }

        public override void Reset ()
        {
            int activeNum = 0;
            for (int i = 0; i < PROBERS_NUM; i++) {
                if (this.probers[i] != null) {
	                this.probers[i].Reset();
	                this.isActive[i] = true;
                    activeNum++;
                } else {
	                this.isActive[i] = false;
                }
            }
	        this.bestGuess = -1;
	        this.state = ProbingState.Detecting;
        }

        public override string GetCharsetName()
        {
            //if we have no answer yet
            if (this.bestGuess == -1) {
	            this.GetConfidence();
                //no charset seems positive
                if (this.bestGuess == -1) this.bestGuess = 0;
            }
            return this.probers[this.bestGuess].GetCharsetName();
        }

    }
}
