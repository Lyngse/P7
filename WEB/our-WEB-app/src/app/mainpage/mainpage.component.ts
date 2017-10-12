import { Component, OnInit } from '@angular/core';
import { APIService } from '../services/api.service';

@Component({
  selector: 'app-mainpage',
  templateUrl: './mainpage.component.html',
  styleUrls: ['./mainpage.component.css']
})
export class MainpageComponent implements OnInit {

  constructor(private apiService: APIService) { }

  getSessionToken(){
    this.apiService.getSessionToken().then(res => {
      console.log(res);
    });
  }

  ngOnInit() {
  }

}
