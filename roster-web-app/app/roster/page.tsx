'use client';

import { FormEvent, useEffect, useMemo, useRef, useState } from 'react';
import FullCalendar from '@fullcalendar/react';
import dayGridPlugin from '@fullcalendar/daygrid';
import timeGridPlugin from '@fullcalendar/timegrid';
import interactionPlugin, { Draggable } from '@fullcalendar/interaction';
import type { EventClickArg, EventDropArg, EventInput } from '@fullcalendar/core';
import type { EventReceiveArg, EventResizeDoneArg } from '@fullcalendar/interaction';
import { CalendarDays, Clock, Loader2, Trash2, X } from 'lucide-react';
import { toast } from 'sonner';

import useRequireAuth from '../hooks/useRequireAuth';
import { getApartments, getCommonAreas, getUnits } from '../services/areaService';
import { getCleaningTasks } from '../services/cleaningTaskService';
import { getHousekeepers } from '../services/housekeeperService';
import { createRoster, getRoster, getRosterByWeek, updateRoster } from '../services/rosterService';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import type {
  Apartment,
  CleaningTask,
  CommonArea,
  Housekeeper,
  Roster,
  RosterAreaType,
  RosterTask,
  Unit,
} from '../../lib/types';

type CleaningArea = {
  id: number;
  name: string;
  areaType: RosterAreaType;
  subtitle: string;
};

type EditForm = {
  taskId: number;
  housekeeperId: number;
  scheduledDate: string;
  startTime: string;
  endTime: string;
  notes: string;
};

const initialEditForm: EditForm = {
  taskId: 0,
  housekeeperId: 0,
  scheduledDate: '',
  startTime: '09:00',
  endTime: '10:00',
  notes: '',
};

export default function RosterPage() {
  useRequireAuth();

  const externalAreasRef = useRef<HTMLDivElement | null>(null);
  const [currentRoster, setCurrentRoster] = useState<Roster | null>(null);
  const [housekeepers, setHousekeepers] = useState<Housekeeper[]>([]);
  const [cleaningTasks, setCleaningTasks] = useState<CleaningTask[]>([]);
  const [areas, setAreas] = useState<CleaningArea[]>([]);
  const [selectedHousekeeperId, setSelectedHousekeeperId] = useState(0);
  const [selectedTaskId, setSelectedTaskId] = useState(0);
  const [weekStartDate, setWeekStartDate] = useState(() => toDateInput(getStartOfWeek(new Date())));
  const [defaultDuration, setDefaultDuration] = useState(60);
  const [areaFilter, setAreaFilter] = useState<RosterAreaType | 'All'>('All');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [editingTask, setEditingTask] = useState<RosterTask | null>(null);
  const [editForm, setEditForm] = useState<EditForm>(initialEditForm);

  useEffect(() => {
    let active = true;

    async function loadBuilderData() {
      try {
        const [housekeeperData, cleaningTaskData, commonAreaData, unitData, apartmentData] = await Promise.all([
          getHousekeepers(),
          getCleaningTasks(),
          getCommonAreas(),
          getUnits(),
          getApartments(),
        ]);

        if (!active) return;

        setHousekeepers(housekeeperData);
        setCleaningTasks(cleaningTaskData);
        setAreas(buildCleaningAreas(commonAreaData, unitData, apartmentData));
        setSelectedHousekeeperId(housekeeperData[0]?.id ?? 0);
        setSelectedTaskId(cleaningTaskData[0]?.id ?? 0);
      } catch (error) {
        toast.error(error instanceof Error ? error.message : 'Failed to load roster builder data.');
      } finally {
        if (active) setLoading(false);
      }
    }

    loadBuilderData();

    return () => {
      active = false;
    };
  }, []);

  useEffect(() => {
    let active = true;

    async function loadRosterForWeek() {
      try {
        const roster = await getRosterByWeek(toApiDate(parseDateInput(weekStartDate)));
        if (active) setCurrentRoster(roster);
      } catch (error) {
        toast.error(error instanceof Error ? error.message : 'Failed to load roster for selected week.');
      }
    }

    loadRosterForWeek();

    return () => {
      active = false;
    };
  }, [weekStartDate]);

  useEffect(() => {
    const container = externalAreasRef.current;
    if (!container) return;

    const draggable = new Draggable(container, {
      itemSelector: '.fc-external-area',
      eventData: (eventEl: HTMLElement) => ({
        title: eventEl.dataset.title ?? 'Cleaning area',
        duration: minutesToDuration(defaultDuration),
        extendedProps: {
          areaId: Number(eventEl.dataset.areaId),
          areaType: eventEl.dataset.areaType as RosterAreaType,
          areaName: eventEl.dataset.areaName ?? '',
        },
      }),
    });

    return () => draggable.destroy();
  }, [defaultDuration, areas]);

  const activeHousekeeper = housekeepers.find((housekeeper) => housekeeper.id === selectedHousekeeperId);
  const selectedCleaningTask = cleaningTasks.find((task) => task.id === selectedTaskId);

  const visibleAreas = useMemo(
    () => areas.filter((area) => areaFilter === 'All' || area.areaType === areaFilter),
    [areas, areaFilter]
  );

  const visibleTasks = useMemo(
    () =>
      currentRoster?.rosterTasks.filter((task) =>
        selectedHousekeeperId ? task.housekeeperId === selectedHousekeeperId : true
      ) ?? [],
    [currentRoster, selectedHousekeeperId]
  );

  const calendarEvents = useMemo(() => mapTasksToEvents(visibleTasks), [visibleTasks]);

  async function saveRosterTasks(tasks: RosterTask[]) {
    const payload: Roster = {
      id: currentRoster?.id ?? 0,
      weekStartDate: toApiDate(parseDateInput(weekStartDate)),
      createdBy: currentRoster?.createdBy || 'Admin',
      createdDate: currentRoster?.createdDate || new Date().toISOString(),
      rosterTasks: tasks.map((task) => ({
        ...task,
        id: task.id > 0 ? task.id : 0,
        rosterId: currentRoster?.id ?? 0,
      })),
    };

    setSaving(true);
    try {
      if (currentRoster?.id) {
        await updateRoster(currentRoster.id, payload);
        setCurrentRoster(await getRoster(currentRoster.id));
      } else {
        const created = await createRoster(payload);
        setCurrentRoster(created);
      }
      toast.success('Roster saved');
    } finally {
      setSaving(false);
    }
  }

  async function handleExternalReceive(info: EventReceiveArg) {
    info.event.remove();

    if (!activeHousekeeper) {
      toast.error('Select a housekeeper before scheduling a task.');
      return;
    }

    if (!selectedCleaningTask) {
      toast.error('Select a cleaning task before scheduling.');
      return;
    }

    if (!info.event.start) {
      toast.error('Drop the area onto a valid timetable slot.');
      return;
    }

    const areaId = Number(info.event.extendedProps.areaId);
    const areaType = info.event.extendedProps.areaType as RosterAreaType;
    const areaName = String(info.event.extendedProps.areaName ?? '');
    const droppedArea = areas.find((area) => area.id === areaId && area.areaType === areaType);

    if (!droppedArea) {
      toast.error('Selected cleaning area is no longer available.');
      return;
    }

    const start = info.event.start;
    const end = info.event.end ?? addMinutes(start, defaultDuration);
    const task = buildRosterTask({
      area: droppedArea,
      cleaningTask: selectedCleaningTask,
      housekeeper: activeHousekeeper,
      scheduledDate: start,
      start,
      end,
      areaName,
    });

    try {
      await saveRosterTasks([...(currentRoster?.rosterTasks ?? []), task]);
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to schedule task.');
    }
  }

  async function handleEventDrop(change: EventDropArg) {
    if (!change.event.start || !change.event.end) return;

    const taskId = Number(change.event.id);
    const updatedTasks = updateTaskTime(currentRoster?.rosterTasks ?? [], taskId, change.event.start, change.event.end);

    try {
      await saveRosterTasks(updatedTasks);
    } catch (error) {
      change.revert();
      toast.error(error instanceof Error ? error.message : 'Failed to move task.');
    }
  }

  async function handleEventResize(change: EventResizeDoneArg) {
    if (!change.event.start || !change.event.end) return;

    const taskId = Number(change.event.id);
    const updatedTasks = updateTaskTime(currentRoster?.rosterTasks ?? [], taskId, change.event.start, change.event.end);

    try {
      await saveRosterTasks(updatedTasks);
    } catch (error) {
      change.revert();
      toast.error(error instanceof Error ? error.message : 'Failed to update task duration.');
    }
  }

  function handleEventClick(click: EventClickArg) {
    const taskId = Number(click.event.id);
    const task = currentRoster?.rosterTasks.find((item) => item.id === taskId);
    if (!task) return;

    setEditingTask(task);
    setEditForm({
      taskId: task.taskId,
      housekeeperId: task.housekeeperId,
      scheduledDate: toDateInput(new Date(task.scheduledDate)),
      startTime: toShortTime(task.startTime),
      endTime: toShortTime(task.endTime),
      notes: task.notes,
    });
  }

  async function handleEditSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (!editingTask) return;

    const start = combineDateTime(editForm.scheduledDate, editForm.startTime);
    const end = combineDateTime(editForm.scheduledDate, editForm.endTime);
    if (start >= end) {
      toast.error('Start time must be before end time.');
      return;
    }

    const selectedTask = cleaningTasks.find((task) => task.id === editForm.taskId);
    const selectedHousekeeper = housekeepers.find((housekeeper) => housekeeper.id === editForm.housekeeperId);
    if (!selectedTask || !selectedHousekeeper) {
      toast.error('Select a valid housekeeper and cleaning task.');
      return;
    }

    const updatedTasks = (currentRoster?.rosterTasks ?? []).map((task) =>
      task.id === editingTask.id
        ? {
            ...task,
            taskId: selectedTask.id,
            taskName: selectedTask.name,
            housekeeperId: selectedHousekeeper.id,
            housekeeperName: selectedHousekeeper.name,
            scheduledDate: toApiDate(start),
            startTime: toTime(start),
            endTime: toTime(end),
            notes: editForm.notes.trim(),
          }
        : task
    );

    try {
      await saveRosterTasks(updatedTasks);
      closeEditDialog();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to update roster task.');
    }
  }

  async function handleDeleteTask() {
    if (!editingTask) return;
    const updatedTasks = (currentRoster?.rosterTasks ?? []).filter((task) => task.id !== editingTask.id);

    try {
      await saveRosterTasks(updatedTasks);
      closeEditDialog();
      toast.success('Roster task removed');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to delete roster task.');
    }
  }

  function closeEditDialog() {
    setEditingTask(null);
    setEditForm(initialEditForm);
  }

  if (loading) {
    return <div className="py-12 text-center">Loading roster builder...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 xl:flex-row xl:items-end xl:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Roster Builder</h1>
          <p className="mt-1 text-gray-600 dark:text-gray-400">
            Build weekly housekeeper timetables by dragging cleaning areas into time slots.
          </p>
        </div>
        <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
          <label className="space-y-2 text-sm font-medium">
            Week
            <Input type="date" value={weekStartDate} onChange={(event) => setWeekStartDate(event.target.value)} />
          </label>
          <label className="space-y-2 text-sm font-medium">
            Housekeeper
            <select
              value={selectedHousekeeperId}
              onChange={(event) => setSelectedHousekeeperId(Number(event.target.value))}
              className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
            >
              <option value={0}>All housekeepers</option>
              {housekeepers.map((housekeeper) => (
                <option key={housekeeper.id} value={housekeeper.id}>
                  {housekeeper.name}
                </option>
              ))}
            </select>
          </label>
          <label className="space-y-2 text-sm font-medium">
            Cleaning task
            <select
              value={selectedTaskId}
              onChange={(event) => setSelectedTaskId(Number(event.target.value))}
              className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
            >
              <option value={0}>Select task</option>
              {cleaningTasks.map((task) => (
                <option key={task.id} value={task.id}>
                  {task.name}
                </option>
              ))}
            </select>
          </label>
          <label className="space-y-2 text-sm font-medium">
            Duration
            <Input
              type="number"
              min={15}
              step={15}
              value={defaultDuration}
              onChange={(event) => setDefaultDuration(Number(event.target.value))}
            />
          </label>
        </div>
      </div>

      <div className="grid gap-4 lg:grid-cols-[320px_minmax(0,1fr)]">
        <Card className="h-fit">
          <CardHeader>
            <CardTitle className="flex items-center gap-2 text-lg">
              <CalendarDays className="h-5 w-5 text-blue-600" />
              Cleaning Areas
            </CardTitle>
            <CardDescription>Drag an area into the timetable to schedule it.</CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-2 gap-2">
              {(['All', 'CommonArea', 'Unit', 'Apartment'] as const).map((filter) => (
                <Button
                  key={filter}
                  type="button"
                  variant={areaFilter === filter ? 'default' : 'outline'}
                  size="sm"
                  onClick={() => setAreaFilter(filter)}
                >
                  {filter === 'CommonArea' ? 'Common' : filter}
                </Button>
              ))}
            </div>

            <div ref={externalAreasRef} className="max-h-[680px] space-y-2 overflow-y-auto pr-1">
              {visibleAreas.length === 0 ? (
                <div className="rounded-md bg-slate-50 px-3 py-4 text-sm text-muted-foreground dark:bg-slate-800">
                  No cleaning areas available.
                </div>
              ) : (
                visibleAreas.map((area) => (
                  <div
                    key={`${area.areaType}-${area.id}`}
                    className="fc-external-area cursor-grab rounded-md border bg-card px-3 py-2 shadow-sm transition hover:border-blue-300 hover:bg-blue-50 active:cursor-grabbing dark:hover:bg-slate-800"
                    data-area-id={area.id}
                    data-area-type={area.areaType}
                    data-area-name={area.name}
                    data-title={`${area.name} - ${area.areaType}`}
                  >
                    <div className="flex items-center justify-between gap-2">
                      <div className="min-w-0">
                        <div className="truncate text-sm font-medium">{area.name}</div>
                        <div className="truncate text-xs text-muted-foreground">{area.subtitle}</div>
                      </div>
                      <Badge variant="outline">{area.areaType}</Badge>
                    </div>
                  </div>
                ))
              )}
            </div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
            <div>
              <CardTitle className="flex items-center gap-2 text-lg">
                <Clock className="h-5 w-5 text-blue-600" />
                Weekly Timetable
              </CardTitle>
              <CardDescription>
                {activeHousekeeper ? `Showing ${activeHousekeeper.name}` : 'Showing all scheduled housekeepers'}
              </CardDescription>
            </div>
            {saving && (
              <Badge variant="outline" className="gap-2">
                <Loader2 className="h-3 w-3 animate-spin" />
                Saving
              </Badge>
            )}
          </CardHeader>
          <CardContent>
            <FullCalendar
              plugins={[dayGridPlugin, timeGridPlugin, interactionPlugin]}
              initialView="timeGridWeek"
              initialDate={weekStartDate}
              key={weekStartDate}
              editable
              droppable
              selectable
              eventResizableFromStart
              allDaySlot={false}
              events={calendarEvents}
              eventReceive={handleExternalReceive}
              eventDrop={handleEventDrop}
              eventResize={handleEventResize}
              eventClick={handleEventClick}
              slotMinTime="06:00:00"
              slotMaxTime="20:00:00"
              slotDuration="00:30:00"
              height="auto"
              headerToolbar={{
                left: 'prev,next today',
                center: 'title',
                right: 'timeGridWeek,dayGridMonth',
              }}
            />
          </CardContent>
        </Card>
      </div>

      {editingTask && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/40 p-4">
          <Card className="max-h-[90vh] w-full max-w-3xl overflow-y-auto">
            <CardHeader>
              <CardTitle className="flex items-center justify-between">
                <span>Edit Roster Task</span>
                <Button variant="ghost" size="icon" onClick={closeEditDialog} aria-label="Close edit roster task">
                  <X className="h-4 w-4" />
                </Button>
              </CardTitle>
              <CardDescription>{editingTask.areaName || editingTask.taskName}</CardDescription>
            </CardHeader>
            <CardContent>
              <form onSubmit={handleEditSubmit} className="grid gap-4 md:grid-cols-2" noValidate>
                <label className="space-y-2 text-sm font-medium">
                  Housekeeper
                  <select
                    value={editForm.housekeeperId}
                    onChange={(event) => setEditForm((current) => ({ ...current, housekeeperId: Number(event.target.value) }))}
                    className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
                  >
                    {housekeepers.map((housekeeper) => (
                      <option key={housekeeper.id} value={housekeeper.id}>
                        {housekeeper.name}
                      </option>
                    ))}
                  </select>
                </label>

                <label className="space-y-2 text-sm font-medium">
                  Cleaning task
                  <select
                    value={editForm.taskId}
                    onChange={(event) => setEditForm((current) => ({ ...current, taskId: Number(event.target.value) }))}
                    className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
                  >
                    {cleaningTasks.map((task) => (
                      <option key={task.id} value={task.id}>
                        {task.name}
                      </option>
                    ))}
                  </select>
                </label>

                <label className="space-y-2 text-sm font-medium">
                  Date
                  <Input
                    type="date"
                    value={editForm.scheduledDate}
                    onChange={(event) => setEditForm((current) => ({ ...current, scheduledDate: event.target.value }))}
                  />
                </label>

                <div className="grid grid-cols-2 gap-3">
                  <label className="space-y-2 text-sm font-medium">
                    Start
                    <Input
                      type="time"
                      value={editForm.startTime}
                      onChange={(event) => setEditForm((current) => ({ ...current, startTime: event.target.value }))}
                    />
                  </label>
                  <label className="space-y-2 text-sm font-medium">
                    End
                    <Input
                      type="time"
                      value={editForm.endTime}
                      onChange={(event) => setEditForm((current) => ({ ...current, endTime: event.target.value }))}
                    />
                  </label>
                </div>

                <label className="space-y-2 text-sm font-medium md:col-span-2">
                  Notes
                  <Textarea
                    value={editForm.notes}
                    onChange={(event) => setEditForm((current) => ({ ...current, notes: event.target.value }))}
                  />
                </label>

                <div className="flex flex-col-reverse gap-2 md:col-span-2 md:flex-row md:justify-between">
                  <Button type="button" variant="destructive" onClick={handleDeleteTask} disabled={saving}>
                    <Trash2 className="h-4 w-4" />
                    Delete
                  </Button>
                  <div className="flex justify-end gap-2">
                    <Button type="button" variant="outline" onClick={closeEditDialog} disabled={saving}>
                      Cancel
                    </Button>
                    <Button type="submit" disabled={saving}>
                      {saving && <Loader2 className="h-4 w-4 animate-spin" />}
                      Save Changes
                    </Button>
                  </div>
                </div>
              </form>
            </CardContent>
          </Card>
        </div>
      )}
    </div>
  );
}

function buildCleaningAreas(commonAreas: CommonArea[], units: Unit[], apartments: Apartment[]): CleaningArea[] {
  return [
    ...commonAreas.map((area) => ({
      id: area.id,
      name: area.name,
      areaType: 'CommonArea' as const,
      subtitle: `${area.locationTypeName} / ${area.buildingBlockName} / ${area.floorName}`,
    })),
    ...units.map((unit) => ({
      id: unit.id,
      name: unit.name,
      areaType: 'Unit' as const,
      subtitle: `${unit.locationTypeName} / ${unit.buildingBlockName} / ${unit.floorName} / ${unit.unitNumber}`,
    })),
    ...apartments.map((apartment) => ({
      id: apartment.id,
      name: apartment.name,
      areaType: 'Apartment' as const,
      subtitle: `${apartment.locationTypeName} / ${apartment.buildingBlockName} / ${apartment.floorName} / ${apartment.apartmentNumber}`,
    })),
  ].sort((a, b) => a.name.localeCompare(b.name));
}

function buildRosterTask({
  area,
  cleaningTask,
  housekeeper,
  scheduledDate,
  start,
  end,
}: {
  area: CleaningArea;
  cleaningTask: CleaningTask;
  housekeeper: Housekeeper;
  scheduledDate: Date;
  start: Date;
  end: Date;
  areaName: string;
}): RosterTask {
  return {
    id: 0,
    rosterId: 0,
    housekeeperId: housekeeper.id,
    housekeeperName: housekeeper.name,
    taskId: cleaningTask.id,
    taskName: cleaningTask.name,
    commonAreaId: area.areaType === 'CommonArea' ? area.id : undefined,
    unitId: area.areaType === 'Unit' ? area.id : undefined,
    apartmentId: area.areaType === 'Apartment' ? area.id : undefined,
    areaType: area.areaType,
    areaName: area.name,
    scheduledDate: toApiDate(scheduledDate),
    startTime: toTime(start),
    endTime: toTime(end),
    frequencyType: cleaningTask.frequency || 'Weekly',
    notes: '',
  };
}

function mapTasksToEvents(tasks: RosterTask[]): EventInput[] {
  return tasks.map((task) => {
    const start = combineDateTime(toDateInput(new Date(task.scheduledDate)), task.startTime);
    const end = combineDateTime(toDateInput(new Date(task.scheduledDate)), task.endTime);

    return {
      id: task.id.toString(),
      title: `${task.areaName || task.taskName} - ${task.housekeeperName}`,
      start,
      end,
      extendedProps: task,
    };
  });
}

function updateTaskTime(tasks: RosterTask[], taskId: number, start: Date, end: Date): RosterTask[] {
  return tasks.map((task) =>
    task.id === taskId
      ? {
          ...task,
          scheduledDate: toApiDate(start),
          startTime: toTime(start),
          endTime: toTime(end),
        }
      : task
  );
}

function getStartOfWeek(date: Date) {
  const copy = new Date(date);
  const day = copy.getDay();
  const diff = day === 0 ? -6 : 1 - day;
  copy.setDate(copy.getDate() + diff);
  copy.setHours(0, 0, 0, 0);
  return copy;
}

function parseDateInput(value: string) {
  return new Date(`${value}T00:00:00`);
}

function toDateInput(date: Date) {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

function toApiDate(date: Date) {
  return `${toDateInput(date)}T00:00:00.000Z`;
}

function toTime(date: Date) {
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');
  return `${hours}:${minutes}:00`;
}

function toShortTime(time: string) {
  return time.slice(0, 5);
}

function combineDateTime(date: string, time: string) {
  return new Date(`${date}T${toShortTime(time)}:00`);
}

function addMinutes(date: Date, minutes: number) {
  return new Date(date.getTime() + minutes * 60 * 1000);
}

function minutesToDuration(minutes: number) {
  const hours = Math.floor(minutes / 60);
  const remainingMinutes = minutes % 60;
  return `${String(hours).padStart(2, '0')}:${String(remainingMinutes).padStart(2, '0')}:00`;
}
