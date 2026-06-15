export interface Housekeeper {
  id: number;
  name: string;
  phone: string;
  email: string;
  status: string;
  employmentType: EmployeeType;
}

export type EmployeeType = 'Permanent' | 'Casual';

export interface CreateHousekeeperRequest {
  name: string;
  phone: string;
  email: string;
  status: string;
  employmentType: EmployeeType;
}

export interface Resident {
  id: number;
  name: string;
  roomNumber: string;
  building: string;
  cleaningFrequency: string;
  notes: string;
}

export interface Location {
  id: number;
  name: string;
  locationType: string;
  building: string;
  floor: string;
  notes: string;
}

export interface CleaningTask {
  id: number;
  name: string;
  description: string;
  estimatedDuration: number;
  frequency: string;
}

export interface RosterTask {
  id: number;
  rosterId: number;
  housekeeperId: number;
  housekeeperName: string;
  taskId: number;
  taskName: string;
  locationId?: number;
  locationName?: string;
  residentId?: number;
  residentName?: string;
  scheduledDate: string;
  startTime: string; // HH:MM format
  endTime: string;
  frequencyType: string;
  notes: string;
}

export interface Roster {
  id: number;
  weekStartDate: string;
  createdBy: string;
  createdDate: string;
  rosterTasks: RosterTask[];
}

