import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';

import { NoteService } from '../../services/note.service';
import { Note } from '../../models/note';

@Component({
  selector: 'wr-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss']
})
export class ToolbarComponent implements OnInit {

  @Output() toggleSidenav = new EventEmitter<void>();

  constructor(private router: Router, private noteService: NoteService) { }

  ngOnInit() {
  }

  createNote(): void {
    const n = new Note();
    n.title = 'New note';
    n.content = '';

    this.noteService.createNote(n)
      .subscribe(createdNote => {
        if (createdNote && createdNote.id) { this.router.navigate(['/edit', createdNote.id]); }
      });
  }

}