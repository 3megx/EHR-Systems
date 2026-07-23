import { Injectable } from '@angular/core';
import { signalStore, withState, withMethods, withComputed, patchState } from '@ngrx/signals';
import { computed } from '@angular/core';
import { Patient } from '../../../core/models';

export interface PatientsState {
  patients: Patient[];
  selectedPatient: Patient | null;
  loading: boolean;
  error: string | null;
  currentPage: number;
  pageSize: number;
  total: number;
  searchTerm: string;
  sortBy: string;
  sortOrder: 'asc' | 'desc';
}

const initialState: PatientsState = {
  patients: [],
  selectedPatient: null,
  loading: false,
  error: null,
  currentPage: 1,
  pageSize: 10,
  total: 0,
  searchTerm: '',
  sortBy: 'firstName',
  sortOrder: 'asc',
};

/**
 * Patient Store
 * Manages patient state using NgRx Signals
 */
@Injectable({
  providedIn: 'root',
})
export class PatientStore extends signalStore(
  { providedIn: 'root' },
  withState<PatientsState>(initialState),
  withComputed(({ patients, currentPage, pageSize }) => ({
    paginatedPatients: computed(() => {
      const start = (currentPage() - 1) * pageSize();
      return patients().slice(start, start + pageSize());
    }),
    totalPages: computed(() => Math.ceil(patients().length / pageSize())),
    hasNextPage: computed(() => currentPage() < Math.ceil(patients().length / pageSize())),
    hasPreviousPage: computed(() => currentPage() > 1),
  })),
  withMethods((store) => ({
    setPatients: (patients: Patient[]) => patchState(store, { patients }),
    setSelectedPatient: (patient: Patient | null) =>
      patchState(store, { selectedPatient: patient }),
    setLoading: (loading: boolean) => patchState(store, { loading }),
    setError: (error: string | null) => patchState(store, { error }),
    setCurrentPage: (page: number) => patchState(store, { currentPage: page }),
    setPageSize: (size: number) => patchState(store, { pageSize: size }),
    setTotal: (total: number) => patchState(store, { total }),
    setSearchTerm: (term: string) => patchState(store, { searchTerm: term }),
    setSortBy: (sortBy: string) => patchState(store, { sortBy }),
    setSortOrder: (order: 'asc' | 'desc') => patchState(store, { sortOrder: order }),
    addPatient: (patient: Patient) =>
      patchState(store, (state) => ({ patients: [...state.patients, patient] })),
    updatePatient: (id: string, updates: Partial<Patient>) =>
      patchState(store, (state) => ({
        patients: state.patients.map((p) => (p.id === id ? { ...p, ...updates } : p)),
      })),
    removePatient: (id: string) =>
      patchState(store, (state) => ({
        patients: state.patients.filter((p) => p.id !== id),
      })),
    reset: () => patchState(store, initialState),
  }))
) {}
