'use client';

export default function RegisterPage() {
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      userName: formData.get('userName'),
      email: formData.get('email'),
      password: formData.get('password'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok) {
        alert('Registration successful! Please check your email to verify your account.');
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Register New Account</h2>
      
      <form onSubmit={handleSubmit}>
        <fieldset>
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
          
          <button type="submit">Register</button>
        </fieldset>
      </form>

      <p>
        Already have an account? <a href="/auth/login">Login here</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response"></pre>
    </>
  );
}
