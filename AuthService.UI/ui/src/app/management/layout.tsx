export default function ManagementLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <>
      <header>
        <h1>AuthService - Management Portal</h1>
        <nav>
          <a href="/management/dashboard">Dashboard</a> |
          <a href="/management/profile">Profile</a> |
          <a href="/management/roles">Roles</a> |
          <a href="/management/groups">Groups</a> |
          <a href="/management/permissions">Permissions</a> |
          <a href="/auth/logout">Logout</a>
        </nav>
      </header>
      <main>
        {children}
      </main>
      <footer>
        <hr />
        <p>&copy; 2025 AuthService - Management Portal</p>
      </footer>
    </>
  );
}
