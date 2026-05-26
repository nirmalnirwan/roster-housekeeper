import type { Metadata } from "next";
import { Inter } from "next/font/google";
import { ThemeProvider } from "next-themes";
import "./globals.css";
import { Toaster } from "sonner";
import Navbar from "@/components/navbar";
import Footer from "@/components/footer";
import SideNav from "@/components/sidenav";

const inter = Inter({ subsets: ["latin"] });

export const metadata: Metadata = {
  title: "Roster Manager - Housekeeper Scheduling",
  description: "Manage weekly cleaning rosters for retirement villages",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
      <html lang="en" suppressHydrationWarning>
        <body className={`${inter.className} min-h-screen flex flex-col`}>
          <ThemeProvider
            attribute="class"
            defaultTheme="system"
            enableSystem
            disableTransitionOnChange
          >
            {/* Navbar */}
            <Navbar />
            {/* Main Content with Sidebar */}
            <div className="flex flex-1">
              <SideNav />
              <main className="flex-1 bg-gray-50 dark:bg-slate-900">
                <div className="container mx-auto px-4 py-8">
                  {children}
                </div>
              </main>
            </div>
            {/* Footer */}
            <Footer />
            <Toaster richColors />
          </ThemeProvider>
        </body>
      </html>
  );
}