'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@clerk/nextjs';
import { Housekeeper } from '../../lib/types';

export default function HousekeepersPage() {
  const [housekeepers, setHousekeepers] = useState<Housekeeper[]>([]);
  const { getToken } = useAuth();

  useEffect(() => {
    async function fetchHousekeepers() {
      const token = await getToken();
      const res = await fetch('http://localhost:5000/api/housekeepers', {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (res.ok) {
        const data = await res.json();
        setHousekeepers(data);
      }
    }
    fetchHousekeepers();
  }, [getToken]);

  return (
    <div>
      <h1>Housekeepers</h1>
      <ul>
        {housekeepers.map(h => (
          <li key={h.id}>{h.name} - {h.email}</li>
        ))}
      </ul>
    </div>
  );
}