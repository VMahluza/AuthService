'use client';

import { useSearchParams, useRouter } from 'next/navigation';
import { useFormState } from 'react-dom';
import { resetPasswordAction, ResetPasswordState } from './action';
import { useEffect } from 'react';

const initialState: ResetPasswordState = {
  success: false,
};

export default function ResetPasswordPage() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const token = searchParams.get('token') || '';
  const [state, formAction] = useFormState(resetPasswordAction, initialState);

  useEffect(() => {
    if (state.success) {
      setTimeout(() => {
        router.push('/auth/login');
      }, 2000);
    }
  }, [state.success, router]);

  return (
    <>
      <h2>Reset Password</h2>
      <p>Enter your new password below.</p>
      
      <form action={formAction}>
        <fieldset>
          <legend>New Password</legend>
          
          <label htmlFor="token">Reset Token:</label>
          <input 
            type="text" 
            id="token" 
            name="token" 
            defaultValue={token}
            required 
          />
          <br /><br />
          
          <label htmlFor="newPassword">New Password:</label>
          <input type="password" id="newPassword" name="newPassword" required />
          <br /><br />
          
          <button type="submit">Reset Password</button>
        </fieldset>
      </form>

      <p>
        <a href="/auth/login">Back to Login</a>
      </p>

      <hr />
      <h3>Response</h3>
      {state.error && <pre style={{ color: 'red' }}>{state.error}</pre>}
      {state.success && <pre style={{ color: 'green' }}>{state.message}</pre>}
      {state.data && <pre>{JSON.stringify(state.data, null, 2)}</pre>}
    </>
  );
}
