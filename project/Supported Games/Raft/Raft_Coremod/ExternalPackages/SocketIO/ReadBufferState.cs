#region License
/*
 * ReadBufferState.cs
 *
 * This code is derived from System.Net.ChunkedInputStream.cs of Mono
 * (https://www.mono-project.com).
 *
 * The MIT License
 *
 * Copyright (c) 2005 Novell, Inc. (https://www.novell.com)
 * Copyright (c) 2014 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

#region Authors
/*
 * Authors:
 * - Gonzalo Paniagua Javier <gonzalo@novell.com>
 */
#endregion

using System;

namespace WebSocketSharp.Net
{
  internal class ReadBufferState
  {
    #region Public Constructors

    public ReadBufferState (
      byte [] buffer, int offset, int count, HttpStreamAsyncResult asyncResult)
    {
      Buffer = buffer;
      Offset = offset;
      Count = count;
      InitialCount = count;
      AsyncResult = asyncResult;
    }

    #endregion

    #region Public Properties

    public HttpStreamAsyncResult AsyncResult {
      get; set;
    }

    public byte [] Buffer {
      get; set;
    }

    public int Count {
      get; set;
    }

    public int InitialCount {
      get; set;
    }

    public int Offset {
      get; set;
    }

    #endregion
  }
}
