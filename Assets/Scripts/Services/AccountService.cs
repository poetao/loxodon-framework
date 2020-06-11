/*
 * MIT License
 *
 * Copyright (c) 2018 Clark Yang
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of 
 * this software and associated documentation files (the "Software"), to deal in 
 * the Software without restriction, including without limitation the rights to 
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
 * of the Software, and to permit persons to whom the Software is furnished to do so, 
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all 
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
 * SOFTWARE.
 */

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Execution;

namespace Game.Services
{
	using Domain		= Domains.Account;
    using IRepository	= Repositories.IAccount;

	public class Account : IAccount
	{
		private IRepository repository;
		private IThreadExecutor executor;

		public Account (IRepository repository)
		{
			this.repository = repository;
			this.executor = new ThreadExecutor ();
		}


		public virtual IAsyncResult<Domain> Register (Domain account)
		{
			return this.repository.Save (account);
		}

		public virtual IAsyncResult<Domain> Login (string username, string password)
		{
			return this.executor.Execute (new Action<IPromise<Domain>> ((promise) => {
				try {
					IAsyncResult<Domain> accountResult = this.GetAccount (username);
					var account = accountResult.Synchronized().WaitForResult ();
					if (account == null || !account.Password.Equals (password)) {
						promise.SetResult (null);
					} else {
						promise.SetResult (account);
					}
				} catch (Exception e) {
					promise.SetException (e);
				}
			}));
		}

		public virtual IAsyncResult<Domain> GetAccount (string username)
		{
			return this.repository.Get (username);
		}
	}
}