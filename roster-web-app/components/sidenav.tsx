'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { Calendar, Users, MapPin, CheckSquare, FileText, Home } from 'lucide-react';

export default function SideNav() {
  const pathname = usePathname();

  const navItems = [
    { href: '/', label: 'Dashboard', icon: Home },
    { href: '/housekeepers', label: 'Housekeepers', icon: Users },
    { href: '/roster', label: 'Weekly Roster', icon: Calendar },
    { href: '/my-schedule', label: 'My Schedule', icon: CheckSquare },
    { href: '/export', label: 'Export', icon: FileText },
  ];

  return (
    <nav className="w-64 bg-gradient-to-b from-blue-900 to-blue-700 text-white shadow-lg">
      <div className="p-6 border-b border-blue-600">
        <h1 className="text-2xl font-bold">Roster Manager</h1>
        <p className="text-sm text-blue-200">Housekeeper Scheduling</p>
      </div>
      <div className="py-4">
        {navItems.map(item => {
          const Icon = item.icon;
          const isActive = pathname === item.href;
          return (
            <Link
              key={item.href}
              href={item.href}
              className={`flex items-center gap-3 px-6 py-3 transition ${
                isActive
                  ? 'bg-blue-600 border-l-4 border-yellow-400'
                  : 'hover:bg-blue-800'
              }`}
            >
              <Icon className="h-5 w-5" />
              <span>{item.label}</span>
            </Link>
          );
        })}
      </div>
    </nav>
  );
}