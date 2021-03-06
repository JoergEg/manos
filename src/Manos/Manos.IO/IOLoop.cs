//
// Copyright (C) 2010 Jackson Harper (jackson@manosdemono.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//



using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Libev;
using Libeio;


namespace Manos.IO {

	public class IOLoop {
	       
		private static IOLoop instance = new IOLoop ();
		
		private bool running;

		private Loop evloop;
		private Libeio.Libeio eio;
		private IntPtr libmanos_data;

		public IOLoop ()
		{
			evloop = Loop.CreateDefaultLoop (0);
			eio = new Libeio.Libeio ();

//			eio.Initialize (evloop);

			libmanos_data = manos_init (evloop.Handle);
		}

		public static IOLoop Instance {
			get { return instance; }
		}

		public Loop EventLoop {
		       get { return evloop; }
		}

		public Libeio.Libeio Eio {
			get { return eio; }
		}

		public void Start ()
		{
			running = true;
			
			evloop.RunBlocking ();
		}

		public void Stop ()
		{
			running = false;
		}

		public void AddTimeout (Timeout timeout)
		{
			TimerWatcher t = new TimerWatcher (timeout.begin, timeout.span, evloop, HandleTimeout);
			t.UserData = timeout;
			t.Start ();
		}

		private void HandleTimeout (Loop loop, TimerWatcher timeout, EventTypes revents)
		{
			Timeout t = (Timeout) timeout.UserData;

			AppHost.RunTimeout (t);
			if (!t.ShouldContinueToRepeat ())
			   timeout.Stop ();
		}

		[DllImport ("libmanos", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr manos_init (IntPtr handle);

	}
}

