'use client';

import { useActionState, useEffect } from 'react';
import { loginAction, LoginState } from '@/app/auth/login/actions';

const initialState: LoginState = {
  message: '',
  error: '',
  accessToken: '',
  refreshToken: '',
  success: false,
};

export default function LoginPage() {
  const [state, formAction, isPending] = useActionState(loginAction, initialState);

  useEffect(() => {
    if (state.success && state.accessToken) {
      localStorage.setItem('accessToken', state.accessToken);
      if (state.refreshToken) {
        localStorage.setItem('refreshToken', state.refreshToken);
      }
      alert('Login successful! Redirecting...');
      window.location.href = '/management/dashboard';
    }
  }, [state]);

  return (
    <>
      <h2>Login to Your Account</h2>
      
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
