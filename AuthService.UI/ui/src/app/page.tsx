import Link from "next/link";

export default function Home() {
  return (
   
      <body>
        <header>
          <h1>AuthService</h1>
          <nav>
            <Link href="/">Home</Link> |
            <Link href="/auth/login">Login</Link> |
            <Link href="/auth/register">Register</Link>
          </nav>
        </header>

        <main>
          <h2>Welcome to AuthService</h2>
          <p>A secure authentication and authorization service.</p>
          
          <hr />
          
          <section>
            <h3>Features</h3>
            <ul>
              <li>User Registration and Email Verification</li>
              <li>Secure Login with JWT Tokens</li>
              <li>Password Reset and Recovery</li>
              <li>Token Refresh Mechanism</li>
              <li>Session Management</li>
              <li>Role-Based Access Control</li>
            </ul>
          </section>
          
          <hr />
          
          <section>
            <h3>Get Started</h3>
            <p>
              <Link href="/auth/register">Create an account</Link> or 
              <Link href="/auth/login">Sign in</Link> to access the management dashboard.
            </p>
          </section>
        </main>

        <footer>
          <hr />
          <p>&copy; 2025 AuthService. All rights reserved.</p>
        </footer>
      </body>
 
       
  );
}
