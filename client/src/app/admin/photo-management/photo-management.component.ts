import { Component, inject, OnInit } from '@angular/core';
import { AdminService } from '../../_services/admin.service';
import { Photo } from '../../_models/photo';

@Component({
  selector: 'app-photo-management',
  standalone: true,
  imports: [],
  templateUrl: './photo-management.component.html',
  styleUrl: './photo-management.component.css'
})
export class PhotoManagementComponent implements OnInit {
  ngOnInit(): void {
    this.getPhotosForApproval();
  }
  photos: Photo[] = [];
  private adminService = inject(AdminService);


  getPhotosForApproval() {
   this.adminService.getPhotosForApproval().subscribe({
    next: photos => this.photos = photos
   })
  }

  approvePhoto(photoId: number) {
    this.adminService.approvePhoto(photoId).subscribe({
      next: () => this.photos.splice(this.photos.findIndex(x => x.id == photoId), 1)
    })
  }

  rejectPhoto(photoId: number) {
    this.adminService.rejectPhoto(photoId).subscribe({
      next: () => this.photos.splice(this.photos.findIndex(p => p.id === photoId), 1)
    })
  }
}
