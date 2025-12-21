import { logoutAction } from './actions';

export default function LogoutPage() {
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
