import { Component, Input, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * File Uploader Component
 * Drag-and-drop file upload component
 * Usage: <app-file-uploader (filesSelected)="onFilesSelected($event)" />
 */
@Component({
  selector: 'app-file-uploader',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div
      (drop)="onDrop($event)"
      (dragover)="onDragOver($event)"
      (dragleave)="onDragLeave()"
      [class.bg-blue-50]="isDragging"
      [class.border-2]="isDragging"
      [class.border-blue-400]="isDragging"
      class="border-2 border-dashed border-gray-300 dark:border-gray-600 rounded-lg p-8 text-center cursor-pointer transition-colors"
    >
      <input
        #fileInput
        type="file"
        [multiple]="multiple"
        [accept]="acceptedFormats"
        (change)="onFileSelected($event)"
        class="hidden"
      />

      <div (click)="fileInput.click()">
        <div class="text-4xl mb-2">📁</div>
        <p class="text-gray-600 dark:text-gray-400 font-medium">
          {{ isDragging ? 'Drop files here' : 'Drag and drop files here' }}
        </p>
        <p class="text-sm text-gray-500 dark:text-gray-500">
          or click to select files
        </p>
        <p *ngIf="acceptedFormats" class="text-xs text-gray-500 dark:text-gray-500 mt-2">
          Accepted: {{ acceptedFormats }}
        </p>
        <p *ngIf="maxSize" class="text-xs text-gray-500 dark:text-gray-500">
          Max size: {{ maxSize }}MB
        </p>
      </div>

      <!-- Selected Files -->
      <div *ngIf="selectedFiles.length > 0" class="mt-4 text-left">
        <p class="text-sm font-medium text-gray-700 dark:text-gray-300 mb-2">Selected files:</p>
        <ul class="space-y-2">
          <li *ngFor="let file of selectedFiles" class="flex items-center justify-between p-2 bg-gray-50 dark:bg-gray-700 rounded">
            <span class="text-sm text-gray-700 dark:text-gray-300">{{ file.name }}</span>
            <button
              (click)="removeFile(file)"
              class="text-red-600 hover:text-red-700 text-sm font-medium"
            >
              ✕
            </button>
          </li>
        </ul>
      </div>
    </div>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class FileUploaderComponent {
  @Input() multiple = true;
  @Input() acceptedFormats = '.pdf,.doc,.docx,.xls,.xlsx,.jpg,.png';
  @Input() maxSize = 10; // MB

  @Output() filesSelected = new EventEmitter<File[]>();

  selectedFiles: File[] = [];
  isDragging = false;

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = true;
  }

  onDragLeave(): void {
    this.isDragging = false;
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging = false;

    const files = event.dataTransfer?.files;
    if (files) {
      this.processFiles(files);
    }
  }

  onFileSelected(event: Event): void {
    const target = event.target as HTMLInputElement;
    const files = target.files;
    if (files) {
      this.processFiles(files);
    }
  }

  private processFiles(files: FileList): void {
    const fileArray = Array.from(files);

    if (!this.multiple) {
      this.selectedFiles = [fileArray[0]];
    } else {
      this.selectedFiles = [...this.selectedFiles, ...fileArray];
    }

    this.filesSelected.emit(this.selectedFiles);
  }

  removeFile(file: File): void {
    this.selectedFiles = this.selectedFiles.filter((f) => f !== file);
    this.filesSelected.emit(this.selectedFiles);
  }
}
