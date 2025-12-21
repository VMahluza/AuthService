export default function AuthLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <header>
        <h1>AuthService - Authentication</h1>
        <nav>
          <a href="/">Home</a> |
          <a href="/auth/login">Login</a> |
          <a href="/auth/register">Register</a> |
          <a href="/auth/forgot-password">Forgot Password</a>
        </nav>
      </header>
      <main>
        {children}
      </main>
      <footer>
        <hr />
        <p>&copy; 2025 AuthService</p>
      </footer>
    </>
  );
}
