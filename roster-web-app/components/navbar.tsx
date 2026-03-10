"use client";

import {  Sparkle } from "lucide-react";
import Link from "next/link";
import ThemeToggle from "./theme-toggle";
import { SignedIn, SignedOut, SignInButton, UserButton } from "@clerk/nextjs";
import { Button } from "./ui/button";

export default function Navbar() {
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
                    {/* Future user profile and auth buttons will go here */}
                    <ThemeToggle/>
                    <SignedOut>
                        <SignInButton>
                            <Button asChild>
                                <Link href="/sign-in">Sign In</Link>
                            </Button>
                        </SignInButton>
                    </SignedOut>
                    <SignedIn>
                       <UserButton />
                    </SignedIn>
                </div>
                
            </div>
        </nav>
    );
}