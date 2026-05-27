"use client";

import {  Sparkle } from "lucide-react";
import Link from "next/link";
import ThemeToggle from "./theme-toggle";
import { Button } from "./ui/button";
import { AUTH_CHANGE_EVENT, isAuthenticated, logout } from "../app/services/authService";
import { useRouter } from "next/navigation";
import { useSyncExternalStore } from "react";

function subscribeToAuthChanges(onStoreChange: () => void) {
    window.addEventListener("storage", onStoreChange);
    window.addEventListener(AUTH_CHANGE_EVENT, onStoreChange);

    return () => {
        window.removeEventListener("storage", onStoreChange);
        window.removeEventListener(AUTH_CHANGE_EVENT, onStoreChange);
    };
}

function getAuthSnapshot() {
    return isAuthenticated();
}

function getServerAuthSnapshot() {
    return false;
}

export default function Navbar() {
    const router = useRouter();
    const auth = useSyncExternalStore(
        subscribeToAuthChanges,
        getAuthSnapshot,
        getServerAuthSnapshot
    );

    const handleLogout = () => {
        logout();
        router.push('/login');
    };

    return (
        <nav className="border-b bg-background">
            <div className="container mx-auto flex h-16 justify-between items-center">
                <div className="flex items-center gap-6">
                    <Link href="/">
                    <div className="flex items-center gap-6">
                        <div>
                            <Sparkle className="h-4 w-4 text-white" />
                        </div>
                        <span className="text-lg font-bold">roster app</span>
                    </div>
                    </Link>
                    <Link href="/roadmap" className="text-sm font-medium text-muted-foreground">
                        Roadmap
                    </Link>
                    <Link href="/feedback" className="text-sm font-medium text-muted-foreground">
                        Feedback
                    </Link>
                </div>
                <div className="flex items-center gap-4">
                    <ThemeToggle/>
                    {auth ? (
                      <Button variant="ghost" onClick={handleLogout}>
                        Logout
                      </Button>
                    ) : (
                      <Button asChild>
                        <Link href="/login">Login</Link>
                      </Button>
                    )}
                </div>
                
            </div>
        </nav>
    );
}
