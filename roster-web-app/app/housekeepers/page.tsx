'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@clerk/nextjs';
import { Housekeeper } from '../../lib/types';
import { Card, CardContent, CardHeader, CardTitle } from '../../components/ui/card';
import { Button } from '../../components/ui/button';
import { Mail, Phone, Briefcase, Badge } from 'lucide-react';

export default function HousekeepersPage() {
  const [housekeepers, setHousekeepers] = useState<Housekeeper[]>([]);
  const [loading, setLoading] = useState(true);
  const { getToken } = useAuth();

  useEffect(() => {
    async function fetchHousekeepers() {
      try {
        const token = await getToken();
        const res = await fetch('http://localhost:5000/api/housekeepers', {
          headers: { Authorization: `Bearer ${token}` }
        });
        if (res.ok) {
          const data = await res.json();
          setHousekeepers(data);
        }
      } catch (error) {
        console.error('Failed to fetch housekeepers', error);
      } finally {
        setLoading(false);
      }
    }
    fetchHousekeepers();
  }, [getToken]);

  if (loading) {
    return <div className="text-center py-12">Loading housekeepers...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold">Housekeepers</h1>
          <p className="text-gray-600 dark:text-gray-400 mt-1">
            Manage your cleaning staff
          </p>
        </div>
        <Button className="bg-blue-600 hover:bg-blue-700">
          Add Housekeeper
        </Button>
      </div>

      <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
        {housekeepers.length === 0 ? (
          <div className="col-span-full text-center py-12 bg-slate-50 dark:bg-slate-800 rounded-lg">
            <p className="text-gray-600 dark:text-gray-400">No housekeepers found</p>
          </div>
        ) : (
          housekeepers.map(h => (
            <Card key={h.id} className="hover:shadow-md transition">
              <CardHeader>
                <CardTitle className="flex items-center justify-between">
                  <span>{h.name}</span>
                  <Badge className={h.status === 'Active' ? 'bg-green-100 text-green-800' : 'bg-gray-100 text-gray-800'}>
                    {h.status}
                  </Badge>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-3">
                <div className="flex items-center gap-2 text-gray-600 dark:text-gray-400">
                  <Mail className="h-4 w-4" />
                  <span className="text-sm">{h.email}</span>
                </div>
                <div className="flex items-center gap-2 text-gray-600 dark:text-gray-400">
                  <Phone className="h-4 w-4" />
                  <span className="text-sm">{h.phone}</span>
                </div>
                <div className="flex items-center gap-2 text-gray-600 dark:text-gray-400">
                  <Briefcase className="h-4 w-4" />
                  <span className="text-sm">{h.employmentType}</span>
                </div>
                <div className="flex gap-2 pt-3">
                  <Button size="sm" variant="outline" className="flex-1">Edit</Button>
                  <Button size="sm" variant="destructive" className="flex-1">Remove</Button>
                </div>
              </CardContent>
            </Card>
          ))
        )}
      </div>
    </div>
  );
}