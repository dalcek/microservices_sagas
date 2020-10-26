import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal, NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { NgForm, FormBuilder, Validators } from '@angular/forms';

@Component({
    selector: 'app-unregisteredDialog',
    templateUrl: './unregisteredDialog.component.html',
    styleUrls: ['./unregisteredDialog.component.css']
})

export class UnregisteredDialogComponent implements OnInit {
    @ViewChild('f') courseForm: NgForm;
    constructor(public activeModal: NgbActiveModal, private fb: FormBuilder) { }
    ngOnInit(): void {
    }
}
