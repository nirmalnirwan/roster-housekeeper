'use client';

import { useEffect, useState } from 'react';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin from '@fullcalendar/interaction';
import { Roster, RosterTask } from '../../lib/types';
import { getRosters, updateRoster } from '../services/rosterService';
import { Button } from '../../components/ui/button';

export default function RosterPage() {
  const [rosters, setRosters] = useState<Roster[]>([]);
  const [current, setCurrent] = useState<Roster | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const data = await getRosters();
        setRosters(data);
        if (data.length > 0) setCurrent(data[0]);
      } catch (error) {
        console.error('Failed to load rosters', error);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, []);

  function mapTasksToEvents(tasks: RosterTask[]) {
    if (!current) return [];
    const weekStart = new Date(current.weekStartDate);
    return tasks.map(t => {
      const [h1, m1] = t.startTime.split(':').map(Number);
      const [h2, m2] = t.endTime.split(':').map(Number);
      const start = new Date(t.scheduledDate);
      start.setHours(h1, m1);
      const end = new Date(t.scheduledDate);
      end.setHours(h2, m2);
      return {
        id: t.id.toString(),
        title: `${t.taskName} - ${t.housekeeperName}`,
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

  if (loading) {
    return <div className="text-center py-12">Loading rosters...</div>;
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold">Weekly Roster</h1>
        <p className="text-gray-600 dark:text-gray-400 mt-1">
          Drag tasks to reschedule them
        </p>
      </div>

      {current && (
        <div className="bg-white dark:bg-slate-800 rounded-lg shadow p-6">
          <FullCalendar
            plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
            initialView="timeGridWeek"
            editable={true}
            selectable={true}
            events={mapTasksToEvents(current.rosterTasks)}
            eventDrop={handleEventDrop}
            slotMinTime="06:00:00"
            slotMaxTime="20:00:00"
            headerToolbar={{
              left: 'prev,next today',
              center: 'title',
              right: 'timeGridWeek,dayGridMonth'
            }}
          />
        </div>
      )}
    </div>
  );
}