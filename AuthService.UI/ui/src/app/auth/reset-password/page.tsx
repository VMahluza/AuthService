'use client';

import { useSearchParams } from 'next/navigation';

export default function ResetPasswordPage() {
  const searchParams = useSearchParams();
  const token = searchParams.get('token') || '';

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      token: formData.get('token'),
      newPassword: formData.get('newPassword'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/reset-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok) {
        alert('Password reset successful! You can now login with your new password.');
        window.location.href = '/auth/login';
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Reset Password</h2>
      <p>Enter your new password below.</p>
      
      <form onSubmit={handleSubmit}>
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
      <pre id="response"></pre>
    </>
  );
}
