import type { Metadata } from "next";
import ManagementLayoutContent from './ManagementLayoutContent';

export const metadata: Metadata = {
  title: "AuthService - Management",
  description: "Management Portal",
};

export default function ManagementLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  return (
    <ManagementLayoutContent>
      {children}
    </ManagementLayoutContent>
  );
}
