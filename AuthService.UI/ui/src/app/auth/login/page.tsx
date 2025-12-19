'use client';

export default function LoginPage() {
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      userName: formData.get('userName'),
      password: formData.get('password'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok && result.accessToken) {
        localStorage.setItem('accessToken', result.accessToken);
        localStorage.setItem('refreshToken', result.refreshToken);
        alert('Login successful! Redirecting...');
        window.location.href = '/management/dashboard';
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Login to Your Account</h2>
      
      <form onSubmit={handleSubmit}>
        <fieldset>
          <legend>Login Credentials</legend>
          
          <label htmlFor="userName">Username:</label>
          <input type="text" id="userName" name="userName" required />
          <br /><br />
          
          <label htmlFor="password">Password:</label>
          <input type="password" id="password" name="password" required />
          <br /><br />
          
          <button type="submit">Login</button>
        </fieldset>
      </form>

      <p>
        Don't have an account? <a href="/auth/register">Register here</a>
      </p>
      <p>
        <a href="/auth/forgot-password">Forgot your password?</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response"></pre>
    </>
  );
}
