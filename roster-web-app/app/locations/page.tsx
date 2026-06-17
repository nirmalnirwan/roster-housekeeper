'use client';

import { FormEvent, useEffect, useMemo, useState } from 'react';
import { Building2, Edit, Layers, Loader2, MapPin, Plus, Trash2, X } from 'lucide-react';
import { toast } from 'sonner';

import useRequireAuth from '../hooks/useRequireAuth';
import {
  createBuildingBlock,
  createFloor,
  deleteBuildingBlock,
  deleteFloor,
  getLocationTypes,
  updateBuildingBlock,
  updateFloor,
} from '@/app/services/locationService';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import type { BuildingBlock, BuildingBlockRequest, Floor, FloorRequest, LocationType } from '@/lib/types';

type BuildingFormState = BuildingBlockRequest;
type FloorFormState = FloorRequest;
type BuildingFormErrors = Partial<Record<keyof BuildingFormState, string>>;
type FloorFormErrors = Partial<Record<keyof FloorFormState, string>>;

const initialBuildingForm: BuildingFormState = {
  name: '',
  locationTypeId: 0,
};

const initialFloorForm: FloorFormState = {
  name: '',
  floorNumber: 1,
  buildingBlockId: 0,
};

export default function LocationsPage() {
  useRequireAuth();

  const [locationTypes, setLocationTypes] = useState<LocationType[]>([]);
  const [loading, setLoading] = useState(true);
  const [loadError, setLoadError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [buildingFormOpen, setBuildingFormOpen] = useState(false);
  const [floorFormOpen, setFloorFormOpen] = useState(false);
  const [editingBuildingId, setEditingBuildingId] = useState<number | null>(null);
  const [editingFloorId, setEditingFloorId] = useState<number | null>(null);
  const [buildingForm, setBuildingForm] = useState<BuildingFormState>(initialBuildingForm);
  const [floorForm, setFloorForm] = useState<FloorFormState>(initialFloorForm);
  const [buildingErrors, setBuildingErrors] = useState<BuildingFormErrors>({});
  const [floorErrors, setFloorErrors] = useState<FloorFormErrors>({});

  useEffect(() => {
    let active = true;

    async function loadLocations() {
      try {
        const data = await getLocationTypes();
        if (active) {
          setLocationTypes(data);
          setLoadError(null);
        }
      } catch (error) {
        console.error('Failed to load locations', error);
        if (active) setLoadError('Failed to load locations. Please try again.');
      } finally {
        if (active) setLoading(false);
      }
    }

    loadLocations();

    return () => {
      active = false;
    };
  }, []);

  const buildingBlocks = useMemo(
    () => locationTypes.flatMap((locationType) => locationType.buildingBlocks),
    [locationTypes]
  );

  const refreshLocations = async () => {
    const data = await getLocationTypes();
    setLocationTypes(data);
  };

  const openCreateBuildingForm = () => {
    setBuildingForm({
      ...initialBuildingForm,
      locationTypeId: locationTypes[0]?.id ?? 0,
    });
    setBuildingErrors({});
    setEditingBuildingId(null);
    setBuildingFormOpen(true);
  };

  const openEditBuildingForm = (buildingBlock: BuildingBlock) => {
    setBuildingForm({
      name: buildingBlock.name,
      locationTypeId: buildingBlock.locationTypeId,
    });
    setBuildingErrors({});
    setEditingBuildingId(buildingBlock.id);
    setBuildingFormOpen(true);
  };

  const closeBuildingForm = () => {
    setBuildingForm(initialBuildingForm);
    setBuildingErrors({});
    setEditingBuildingId(null);
    setBuildingFormOpen(false);
  };

  const openCreateFloorForm = (buildingBlockId?: number) => {
    setFloorForm({
      ...initialFloorForm,
      buildingBlockId: buildingBlockId ?? buildingBlocks[0]?.id ?? 0,
    });
    setFloorErrors({});
    setEditingFloorId(null);
    setFloorFormOpen(true);
  };

  const openEditFloorForm = (floor: Floor) => {
    setFloorForm({
      name: floor.name,
      floorNumber: floor.floorNumber,
      buildingBlockId: floor.buildingBlockId,
    });
    setFloorErrors({});
    setEditingFloorId(floor.id);
    setFloorFormOpen(true);
  };

  const closeFloorForm = () => {
    setFloorForm(initialFloorForm);
    setFloorErrors({});
    setEditingFloorId(null);
    setFloorFormOpen(false);
  };

  const handleBuildingSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const normalized = {
      ...buildingForm,
      name: buildingForm.name.trim(),
    };
    const validationErrors = validateBuildingForm(normalized);
    setBuildingErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    setSubmitting(true);

    try {
      if (editingBuildingId) {
        await updateBuildingBlock(editingBuildingId, normalized);
        toast.success('Building block updated');
      } else {
        await createBuildingBlock(normalized);
        toast.success('Building block created');
      }

      await refreshLocations();
      closeBuildingForm();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to save building block.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleFloorSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();

    const normalized = {
      ...floorForm,
      name: floorForm.name.trim(),
      floorNumber: Number(floorForm.floorNumber),
    };
    const validationErrors = validateFloorForm(normalized);
    setFloorErrors(validationErrors);
    if (Object.keys(validationErrors).length > 0) return;

    setSubmitting(true);

    try {
      if (editingFloorId) {
        await updateFloor(editingFloorId, normalized);
        toast.success('Floor updated');
      } else {
        await createFloor(normalized);
        toast.success('Floor created');
      }

      await refreshLocations();
      closeFloorForm();
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to save floor.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteBuilding = async (buildingBlock: BuildingBlock) => {
    setSubmitting(true);
    try {
      await deleteBuildingBlock(buildingBlock.id);
      await refreshLocations();
      toast.success('Building block removed');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to remove building block.');
    } finally {
      setSubmitting(false);
    }
  };

  const handleDeleteFloor = async (floor: Floor) => {
    setSubmitting(true);
    try {
      await deleteFloor(floor.id);
      await refreshLocations();
      toast.success('Floor removed');
    } catch (error) {
      toast.error(error instanceof Error ? error.message : 'Failed to remove floor.');
    } finally {
      setSubmitting(false);
    }
  };

  if (loading) {
    return <div className="py-12 text-center">Loading locations...</div>;
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-3xl font-bold">Locations</h1>
          <p className="mt-1 text-gray-600 dark:text-gray-400">
            Manage location types, building blocks, and floors
          </p>
        </div>
        <div className="flex flex-wrap gap-2">
          <Button variant="outline" onClick={() => openCreateFloorForm()} disabled={buildingBlocks.length === 0}>
            <Layers className="h-4 w-4" />
            Add Floor
          </Button>
          <Button className="bg-blue-600 hover:bg-blue-700" onClick={openCreateBuildingForm}>
            <Plus className="h-4 w-4" />
            Add Building Block
          </Button>
        </div>
      </div>

      {loadError && (
        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
          {loadError}
        </div>
      )}

      {buildingFormOpen && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              <span>{editingBuildingId ? 'Edit Building Block' : 'Add Building Block'}</span>
              <Button variant="ghost" size="icon" onClick={closeBuildingForm} aria-label="Close building form">
                <X className="h-4 w-4" />
              </Button>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleBuildingSubmit} className="grid gap-4 md:grid-cols-2" noValidate>
              <Field label="Location type" error={buildingErrors.locationTypeId}>
                <select
                  value={buildingForm.locationTypeId}
                  onChange={(event) =>
                    setBuildingForm((current) => ({
                      ...current,
                      locationTypeId: Number(event.target.value),
                    }))
                  }
                  className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
                  aria-invalid={Boolean(buildingErrors.locationTypeId)}
                  disabled={submitting}
                >
                  <option value={0}>Select location type</option>
                  {locationTypes.map((locationType) => (
                    <option key={locationType.id} value={locationType.id}>
                      {locationType.name}
                    </option>
                  ))}
                </select>
              </Field>

              <Field label="Building block name" error={buildingErrors.name}>
                <Input
                  value={buildingForm.name}
                  onChange={(event) =>
                    setBuildingForm((current) => ({ ...current, name: event.target.value }))
                  }
                  placeholder="Building Block A"
                  aria-invalid={Boolean(buildingErrors.name)}
                  disabled={submitting}
                />
              </Field>

              <div className="flex justify-end gap-2 md:col-span-2">
                <Button type="button" variant="outline" onClick={closeBuildingForm} disabled={submitting}>
                  Cancel
                </Button>
                <Button type="submit" disabled={submitting}>
                  {submitting && <Loader2 className="h-4 w-4 animate-spin" />}
                  {editingBuildingId ? 'Save Changes' : 'Create Building Block'}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      {floorFormOpen && (
        <Card>
          <CardHeader>
            <CardTitle className="flex items-center justify-between">
              <span>{editingFloorId ? 'Edit Floor' : 'Add Floor'}</span>
              <Button variant="ghost" size="icon" onClick={closeFloorForm} aria-label="Close floor form">
                <X className="h-4 w-4" />
              </Button>
            </CardTitle>
          </CardHeader>
          <CardContent>
            <form onSubmit={handleFloorSubmit} className="grid gap-4 md:grid-cols-3" noValidate>
              <Field label="Building block" error={floorErrors.buildingBlockId}>
                <select
                  value={floorForm.buildingBlockId}
                  onChange={(event) =>
                    setFloorForm((current) => ({
                      ...current,
                      buildingBlockId: Number(event.target.value),
                    }))
                  }
                  className="border-input focus-visible:border-ring focus-visible:ring-ring/50 h-9 w-full rounded-md border bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:ring-[3px]"
                  aria-invalid={Boolean(floorErrors.buildingBlockId)}
                  disabled={submitting}
                >
                  <option value={0}>Select building block</option>
                  {buildingBlocks.map((buildingBlock) => (
                    <option key={buildingBlock.id} value={buildingBlock.id}>
                      {buildingBlock.locationTypeName} - {buildingBlock.name}
                    </option>
                  ))}
                </select>
              </Field>

              <Field label="Floor name" error={floorErrors.name}>
                <Input
                  value={floorForm.name}
                  onChange={(event) =>
                    setFloorForm((current) => ({ ...current, name: event.target.value }))
                  }
                  placeholder="Floor 1"
                  aria-invalid={Boolean(floorErrors.name)}
                  disabled={submitting}
                />
              </Field>

              <Field label="Floor number" error={floorErrors.floorNumber}>
                <Input
                  type="number"
                  min={0}
                  max={200}
                  value={floorForm.floorNumber}
                  onChange={(event) =>
                    setFloorForm((current) => ({
                      ...current,
                      floorNumber: Number(event.target.value),
                    }))
                  }
                  aria-invalid={Boolean(floorErrors.floorNumber)}
                  disabled={submitting}
                />
              </Field>

              <div className="flex justify-end gap-2 md:col-span-3">
                <Button type="button" variant="outline" onClick={closeFloorForm} disabled={submitting}>
                  Cancel
                </Button>
                <Button type="submit" disabled={submitting}>
                  {submitting && <Loader2 className="h-4 w-4 animate-spin" />}
                  {editingFloorId ? 'Save Changes' : 'Create Floor'}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      )}

      <div className="space-y-5">
        {locationTypes.map((locationType) => (
          <section key={locationType.id} className="space-y-3">
            <div className="flex items-center gap-3">
              <MapPin className="h-5 w-5 text-blue-600" />
              <h2 className="text-xl font-semibold">{locationType.name}</h2>
              <Badge variant="outline">{locationType.buildingBlocks.length} blocks</Badge>
            </div>

            {locationType.buildingBlocks.length === 0 ? (
              <div className="rounded-lg bg-slate-50 px-4 py-6 text-sm text-gray-600 dark:bg-slate-800 dark:text-gray-400">
                No building blocks have been added for {locationType.name}.
              </div>
            ) : (
              <div className="grid gap-4 lg:grid-cols-2">
                {locationType.buildingBlocks.map((buildingBlock) => (
                  <Card key={buildingBlock.id} className="transition hover:shadow-md">
                    <CardHeader>
                      <CardTitle className="flex items-center justify-between gap-3">
                        <span className="flex items-center gap-2">
                          <Building2 className="h-5 w-5" />
                          {buildingBlock.name}
                        </span>
                        <span className="flex gap-1">
                          <Button size="icon-sm" variant="ghost" onClick={() => openEditBuildingForm(buildingBlock)} disabled={submitting} aria-label="Edit building block">
                            <Edit className="h-4 w-4" />
                          </Button>
                          <Button size="icon-sm" variant="ghost" onClick={() => handleDeleteBuilding(buildingBlock)} disabled={submitting} aria-label="Delete building block">
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </span>
                      </CardTitle>
                    </CardHeader>
                    <CardContent className="space-y-3">
                      <div className="flex items-center justify-between">
                        <div className="text-sm text-muted-foreground">Floors</div>
                        <Button size="sm" variant="outline" onClick={() => openCreateFloorForm(buildingBlock.id)} disabled={submitting}>
                          <Plus className="h-4 w-4" />
                          Add Floor
                        </Button>
                      </div>

                      {buildingBlock.floors.length === 0 ? (
                        <p className="rounded-md bg-slate-50 px-3 py-4 text-sm text-gray-600 dark:bg-slate-800 dark:text-gray-400">
                          No floors added.
                        </p>
                      ) : (
                        <div className="space-y-2">
                          {buildingBlock.floors.map((floor) => (
                            <div
                              key={floor.id}
                              className="flex items-center justify-between rounded-md border px-3 py-2"
                            >
                              <div>
                                <div className="font-medium">{floor.name}</div>
                                <div className="text-sm text-muted-foreground">
                                  Floor number {floor.floorNumber}
                                </div>
                              </div>
                              <div className="flex gap-1">
                                <Button size="icon-sm" variant="ghost" onClick={() => openEditFloorForm(floor)} disabled={submitting} aria-label="Edit floor">
                                  <Edit className="h-4 w-4" />
                                </Button>
                                <Button size="icon-sm" variant="ghost" onClick={() => handleDeleteFloor(floor)} disabled={submitting} aria-label="Delete floor">
                                  <Trash2 className="h-4 w-4" />
                                </Button>
                              </div>
                            </div>
                          ))}
                        </div>
                      )}
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}
          </section>
        ))}
      </div>
    </div>
  );
}

function Field({
  label,
  error,
  children,
}: {
  label: string;
  error?: string;
  children: React.ReactNode;
}) {
  return (
    <label className="space-y-2">
      <span className="text-sm font-medium">{label}</span>
      {children}
      {error && <span className="block text-sm text-destructive">{error}</span>}
    </label>
  );
}

function validateBuildingForm(form: BuildingFormState): BuildingFormErrors {
  const errors: BuildingFormErrors = {};

  if (!form.locationTypeId) errors.locationTypeId = 'Location type is required.';
  if (!form.name) errors.name = 'Building block name is required.';

  return errors;
}

function validateFloorForm(form: FloorFormState): FloorFormErrors {
  const errors: FloorFormErrors = {};

  if (!form.buildingBlockId) errors.buildingBlockId = 'Building block is required.';
  if (!form.name) errors.name = 'Floor name is required.';
  if (!Number.isFinite(form.floorNumber) || form.floorNumber < 0) {
    errors.floorNumber = 'Floor number is required.';
  }

  return errors;
}
