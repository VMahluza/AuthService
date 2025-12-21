'use client';

import { logoutAction } from './actions';
import { useEffect } from 'react';
import { clearTokens } from '@/lib/token-utils';

export default function LogoutPage() {
  // Clear client-side tokens on mount
  useEffect(() => {
    clearTokens();
  }, []);

  return (
    <>
      <h2>Logout</h2>
      <p>Sign out from your account.</p>
      
      <form action={logoutAction}>
        <fieldset>
          <legend>Logout Options</legend>
          
          <label htmlFor="revokeAllSessions">
            <input 
              type="checkbox" 
              id="revokeAllSessions" 
              name="revokeAllSessions" 
            />
            Revoke all sessions
          </label>
          <br /><br />
          
          <button type="submit">Logout</button>
        </fieldset>
      </form>

      <p>
        <a href="/management/dashboard">Back to Dashboard</a>
      </p>
    </>
  );
}
