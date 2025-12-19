'use client';

export default function ForgotPasswordPage() {
  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    const formData = new FormData(e.currentTarget);
    const data = {
      email: formData.get('email'),
    };

    try {
      const response = await fetch('http://localhost:5102/api/auth/forgot-password', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(data),
      });

      const result = await response.json();
      document.getElementById('response')!.textContent = JSON.stringify(result, null, 2);

      if (response.ok) {
        alert('Password reset link sent! Please check your email.');
      }
    } catch (error: any) {
      document.getElementById('response')!.textContent = 'Error: ' + error.message;
    }
  };

  return (
    <>
      <h2>Forgot Password</h2>
      <p>Enter your email address to receive a password reset link.</p>
      
      <form onSubmit={handleSubmit}>
        <fieldset>
          <legend>Email Address</legend>
          
          <label htmlFor="email">Email:</label>
          <input type="email" id="email" name="email" required />
          <br /><br />
          
          <button type="submit">Send Reset Link</button>
        </fieldset>
      </form>

      <p>
        Remember your password? <a href="/auth/login">Login here</a>
      </p>

      <hr />
      <h3>Response</h3>
      <pre id="response"></pre>
    </>
  );
}
