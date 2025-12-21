'use client';

import { useActionState, useEffect } from 'react';
import { loginAction, LoginState } from '@/app/auth/login/actions';
import { useRouter } from 'next/navigation';

const initialState: LoginState = {
  message: '',
  error: '',
  success: false,
};

export default function LoginPage() {
  const [state, formAction, isPending] = useActionState(loginAction, initialState);
  const router = useRouter();

  // Sync tokens to localStorage on successful login
  useEffect(() => {
    if (state.success && state.tokens) {
      localStorage.setItem('accessToken', state.tokens.accessToken);
      localStorage.setItem('refreshToken', state.tokens.refreshToken);
      // Redirect after storing tokens
      router.push('/management/dashboard');
    }
  }, [state.success, state.tokens, router]);

  return (
    <>
      <h2>Login to Your Account</h2>
      {state.error && <p className="text-red-500 mb-4">{state.error}</p>}
      
      <form action={formAction}>
        <fieldset disabled={isPending}>
          <legend>Login Credentials</legend>
          
          <label htmlFor="userName">Username:</label>
          <input type="text" id="userName" name="userName" required />
          <br /><br />
          
          <label htmlFor="password">Password:</label>
          <input type="password" id="password" name="password" required />
          <br /><br />
          
          <button type="submit">{isPending ? 'Logging in...' : 'Login'}</button>
        </fieldset>
      </form>

      <p>
        Don&apos;t have an account? <a href="/auth/register">Register here</a>
      </p>
      <p>
        <a href="/auth/forgot-password">Forgot your password?</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response">
        {state.error ? `Error: ${state.error}` : (state.success ? JSON.stringify(state, null, 2) : '')}
      </pre>
    </>
  );
}
