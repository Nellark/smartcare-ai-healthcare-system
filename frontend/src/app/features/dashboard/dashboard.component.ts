import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface DashboardPatient {
  id: string;
  name: string;
  dateOfBirth: string;
  gender: 'Male' | 'Female';
  lastVisit: string;
  status: 'Active' | 'Pending' | 'Inactive';
  avatarUrl: string;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit {
  searchQuery = '';
  statusFilter = 'All';
  physicianFilter = 'All';
  
  isModalOpen = false;

  currentPage = 1;
  pageSize = 10;
  totalEntries = 1240;

  activePatients = 842;
  inPatients = 12;
  totalRecords = 1240;
  pendingReviews = 24;

  allPatients: DashboardPatient[] = [
    { id: 'SC-8921', name: 'Martha Jenkins',   dateOfBirth: 'May 12, 1954', gender: 'Female', lastVisit: 'Oct 24, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=47' },
    { id: 'SC-9042', name: 'Alejandro Silva',   dateOfBirth: 'Jan 18, 1991', gender: 'Male',   lastVisit: 'Nov 02, 2023', status: 'Pending',  avatarUrl: 'https://i.pravatar.cc/150?img=53' },
    { id: 'SC-7731', name: 'Sarah Johnson',     dateOfBirth: 'Nov 30, 1985', gender: 'Female', lastVisit: 'Aug 15, 2023', status: 'Inactive', avatarUrl: '' },
    { id: 'SC-1240', name: 'Li Wei Chen',       dateOfBirth: 'Jul 04, 1970', gender: 'Male',   lastVisit: 'Nov 14, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=61' },
    { id: 'SC-3305', name: 'Emily Davis',       dateOfBirth: 'Mar 22, 1978', gender: 'Female', lastVisit: 'Sep 30, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=45' },
    { id: 'SC-4471', name: 'James Okafor',      dateOfBirth: 'Jun 09, 1965', gender: 'Male',   lastVisit: 'Oct 11, 2023', status: 'Pending',  avatarUrl: 'https://i.pravatar.cc/150?img=12' },
    { id: 'SC-6612', name: 'Priya Sharma',      dateOfBirth: 'Feb 14, 1992', gender: 'Female', lastVisit: 'Nov 01, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=49' },
    { id: 'SC-2288', name: 'Robert Kim',        dateOfBirth: 'Sep 03, 1982', gender: 'Male',   lastVisit: 'Jul 22, 2023', status: 'Inactive', avatarUrl: 'https://i.pravatar.cc/150?img=14' },
    { id: 'SC-5190', name: 'Fatima Al-Hassan',  dateOfBirth: 'Dec 19, 1975', gender: 'Female', lastVisit: 'Nov 08, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=44' },
    { id: 'SC-7803', name: 'Carlos Mendez',     dateOfBirth: 'Aug 27, 1989', gender: 'Male',   lastVisit: 'Oct 05, 2023', status: 'Active',   avatarUrl: 'https://i.pravatar.cc/150?img=15' },
  ];

  get filteredPatients(): DashboardPatient[] {
    return this.allPatients.filter(p => {
      const matchesSearch = !this.searchQuery ||
        p.name.toLowerCase().includes(this.searchQuery.toLowerCase()) ||
        p.id.toLowerCase().includes(this.searchQuery.toLowerCase());
      const matchesStatus = this.statusFilter === 'All' || p.status === this.statusFilter;
      return matchesSearch && matchesStatus;
    });
  }

  get totalPages(): number {
    return Math.ceil(this.totalEntries / this.pageSize);
  }

  get displayedFrom(): number {
    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get displayedTo(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalEntries);
  }

  get pageNumbers(): (number | string)[] {
    const pages: (number | string)[] = [];
    const total = this.totalPages;
    if (total <= 5) {
      for (let i = 1; i <= total; i++) pages.push(i);
    } else {
      pages.push(1, 2, 3);
      if (this.currentPage > 5) pages.push('...');
      if (this.currentPage > 4 && this.currentPage < total - 2) pages.push(this.currentPage);
      pages.push('...');
      pages.push(total);
    }
    return pages;
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).slice(0, 2).join('').toUpperCase();
  }

  ngOnInit(): void {}

  openModal(): void {
    this.isModalOpen = true;
  }

  closeModal(): void {
    this.isModalOpen = false;
  }

  setPage(page: number | string): void {
    if (typeof page === 'number' && page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
    }
  }

  prevPage(): void {
    if (this.currentPage > 1) this.currentPage--;
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) this.currentPage++;
  }
}
