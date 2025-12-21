'use client';

import { useActionState } from 'react';
import { registerAction, RegisterState } from './actions';

const initialState: RegisterState = {
  message: '',
  error: '',
  success: false,
};

export default function RegisterPage() {
  const [state, formAction, isPending] = useActionState(registerAction, initialState);

  return (
    <>
      <h2>Register New Account</h2>
      
      {state.error && <p className="text-red-500 mb-4">{state.error}</p>}
      {state.success && <p className="text-green-500 mb-4">{state.message}</p>}

      <form action={formAction}>
        <fieldset disabled={isPending}>
          <legend>Account Information</legend>
          
          <label htmlFor="userName">Username:</label>
          <input type="text" id="userName" name="userName" required />
          <br /><br />
          
          <label htmlFor="email">Email:</label>
          <input type="email" id="email" name="email" required />
          <br /><br />
          
          <label htmlFor="password">Password:</label>
          <input type="password" id="password" name="password" required />
          <br /><br />
          
          <button type="submit">{isPending ? 'Registering...' : 'Register'}</button>
        </fieldset>
      </form>

      <p>
        Already have an account? <a href="/auth/login">Login here</a>
      </p>
    </>
  );
}
