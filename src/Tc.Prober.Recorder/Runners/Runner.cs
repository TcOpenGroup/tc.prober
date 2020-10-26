﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vortex.Connector;

namespace Tc.Prober.Recorder
{
    public static class Runner
    {       
        public static string RecordingsShell { get; set; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string CallerMethodName(int frame = 1)
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(frame);

            return sf.GetMethod().Name;
        }

        public static string GetAutoMethodName(int level = 2)
        {
            return Path.Combine(RecordingsShell, $"{CallerMethodName(level)}.json");
        }
    

        private static string GetAutoName()
        {
            return Path.Combine(RecordingsShell, $"{CallerMethodName(3)}.json");
        }
        
        public static T Run<T>(this IVortexElement sut, 
                                   Func<T> action,
                                   Func<bool> done,                                   
                                   Action openCycle = null,
                                   Action closeCycle = null,                                   
                                   IRecorder recorder = null,
                                   string recordingFileName = null
                                   )
                            
        {                               
            T retVal = default(T);

            recorder?.Begin(recordingFileName);

            while (done())
            {
                recorder?.Act();
                openCycle?.Invoke();
                retVal = action();
                closeCycle?.Invoke();
                recorder?.Act();
            }

            recorder?.Act();

            recorder?.End(recordingFileName);

            return retVal;
        }

        
    }
}