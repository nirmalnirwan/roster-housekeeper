'use client';

import { useEffect, useState } from 'react';
import { useAuth } from '@clerk/nextjs';
import { Housekeeper, RosterTask } from '../../lib/types';
import { getRosters } from '../services/rosterService';

export default function HousekeeperSchedulePage() {
  const [tasks, setTasks] = useState<RosterTask[]>([]);
  const { getToken } = useAuth();

  useEffect(() => {
    async function load() {
      const token = await getToken();
      // In a real app, we'd fetch housekeeper ID from context/session
      const rosters = await getRosters();
      const allTasks = rosters.flatMap(r => r.rosterTasks);
      setTasks(allTasks);
    }
    load();
  }, [getToken]);

  // Group tasks by day
  const tasksByDay: Record<string, RosterTask[]> = {};
  tasks.forEach(t => {
    const day = new Date(t.scheduledDate).toLocaleDateString('en-US', { weekday: 'long' });
    if (!tasksByDay[day]) tasksByDay[day] = [];
    tasksByDay[day].push(t);
  });

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-6">My Schedule</h1>
      {Object.entries(tasksByDay).map(([day, dayTasks]) => (
        <div key={day} className="mb-8">
          <h2 className="text-xl font-semibold mb-3">{day}</h2>
          <div className="space-y-2">
            {dayTasks
              .sort((a, b) => a.startTime.localeCompare(b.startTime))
              .map(t => (
                <div key={t.id} className="p-3 bg-blue-100 rounded border border-blue-300">
                  <div className="font-semibold">{t.startTime} - {t.endTime}</div>
                  <div className="text-sm">{t.taskName}</div>
                  {t.locationName && <div className="text-xs text-gray-600">{t.locationName}</div>}
                  {t.residentName && <div className="text-xs text-gray-600">Resident: {t.residentName}</div>}
                </div>
              ))}
          </div>
        </div>
      ))}
    </div>
  );
}