'use client';

import { forgotPasswordAction, ForgotPasswordState } from './action';
import Link from 'next/link';
import { useActionState } from 'react';

const initialState: ForgotPasswordState = {
  message: '',
  error: '',
  success: false,
  data: undefined
};

export default function ForgotPasswordPage() {
  const [state, formAction] = useActionState(forgotPasswordAction, initialState);

  return (
    <>
      <h2>Forgot Password</h2>
      <p>Enter your email address to receive a password reset link.</p>
      
      <form action={formAction}>
        <fieldset>
          <legend>Email Address</legend>
          
          <label htmlFor="email">Email:</label>
          <input type="email" id="email" name="email" required />
          <br /><br />
          
          <button type="submit">Send Reset Link</button>
        </fieldset>
      </form>

      {state.success && (
          <p style={{ color: 'green' }}>{state.message}</p>
      )}
      {state.error && (
          <p style={{ color: 'red' }}>{state.error}</p>
      )}

      <p>
        Remember your password? <Link href="/auth/login">Login here</Link>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response">{JSON.stringify(state, null, 2)}</pre>
    </>
  );
}
