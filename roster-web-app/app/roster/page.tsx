'use client';

import { useEffect, useState } from 'react';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { Roster, RosterTask } from '../../lib/types';
import { getRosters, updateRoster } from '../services/rosterService';

export default function RosterPage() {
  const [rosters, setRosters] = useState<Roster[]>([]);
  const [current, setCurrent] = useState<Roster | null>(null);

  useEffect(() => {
    async function load() {
      const data = await getRosters();
      setRosters(data);
      if (data.length > 0) setCurrent(data[0]);
    }
    load();
  }, []);

  function mapTasksToEvents(tasks: RosterTask[]) {
    if (!current) return [];
    const weekStart = new Date(current.weekStartDate);
    return tasks.map(t => {
      const [h1, m1] = t.startTime.split(':').map(Number);
      const [h2, m2] = t.endTime.split(':').map(Number);
      const dayOffset = new Date(t.startTime).getDay(); // placeholder
      // actually tasks don't have day info; assume startTime includes full date?
      // For simplicity we append to weekStart + some offset if stored in notes?
      const start = new Date(weekStart);
      start.setHours(h1, m1);
      const end = new Date(weekStart);
      end.setHours(h2, m2);
      return {
        id: t.id.toString(),
        title: t.taskName,
        start,
        end,
        extendedProps: t,
      };
    });
  }

  function handleEventDrop(change: any) {
    const { event } = change;
    if (!current) return;
    const taskId = parseInt(event.id, 10);
    const task = current.rosterTasks.find(t => t.id === taskId);
    if (!task) return;
    task.startTime = event.start.toISOString().substring(11, 16);
    task.endTime = event.end.toISOString().substring(11, 16);
    updateRoster(current.id, current);
  }

  return (
    <div className="p-4">
      <h1 className="text-2xl font-bold mb-4">Weekly Roster</h1>
      {current && (
        <FullCalendar
          plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
          initialView="timeGridWeek"
          editable={true}
          selectable={true}
          events={mapTasksToEvents(current.rosterTasks)}
          eventDrop={handleEventDrop}
          slotMinTime="06:00:00"
          slotMaxTime="20:00:00"
        />
      )}
    </div>
  );
}