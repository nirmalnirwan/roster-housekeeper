'use client';

import { useEffect, useState } from 'react';
import { Roster } from '../../lib/types';
import { getRosters, exportRosterPdf, exportRosterExcel } from '../services/rosterService';
import { Button } from '../../components/ui/button';

export default function ExportPage() {
  const [rosters, setRosters] = useState<Roster[]>([]);

  useEffect(() => {
    async function load() {
      const data = await getRosters();
      setRosters(data);
    }
    load();
  }, []);

  async function handleExportPdf(rosterId: number) {
    const blob = await exportRosterPdf(rosterId);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `roster_${rosterId}.pdf`;
    a.click();
  }

  async function handleExportExcel(rosterId: number) {
    const blob = await exportRosterExcel(rosterId);
    const url = window.URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = `roster_${rosterId}.xlsx`;
    a.click();
  }

  return (
    <div className="p-6">
      <h1 className="text-3xl font-bold mb-6">Export Rosters</h1>
      <div className="space-y-4">
        {rosters.map(r => (
          <div key={r.id} className="p-4 border rounded bg-gray-50">
            <div className="font-semibold">
              Week of {new Date(r.weekStartDate).toLocaleDateString()}
            </div>
            <div className="text-sm text-gray-600 mb-3">
              {r.rosterTasks.length} tasks
            </div>
            <div className="flex gap-2">
              <Button onClick={() => handleExportPdf(r.id)} className="bg-red-500">
                Export PDF
              </Button>
              <Button onClick={() => handleExportExcel(r.id)} className="bg-green-500">
                Export Excel
              </Button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}