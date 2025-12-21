import type { Metadata } from "next";
import { Inter, Roboto } from 'next/font/google';
import './globals.css';

// Define fonts with the weights and styles from your original link
const inter = Inter({
  subsets: ['latin'],
  weight: ['100', '300', '400', '500', '700', '900'],
  style: ['normal', 'italic'],
  variable: '--font-inter', // Optional: for CSS variables
});

const roboto = Roboto({
  subsets: ['latin'],
  weight: ['100', '300', '400', '500', '700', '900'],
  style: ['normal', 'italic'],
  variable: '--font-roboto', // Optional: for CSS variables
});

export const metadata: Metadata = {
  title: "MarketingPage Template - Material UI",
  description: "Authentication Service",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
      <head>
        {/* Icons to support Material Design */}
        <link
          rel="stylesheet"
          href="https://fonts.googleapis.com/icon?family=Material+Icons"
        />
      </head>
      <body className={`${inter.variable} ${roboto.variable}`}>
        {children}
      </body>
    </html>
  );
}
