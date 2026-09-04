import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { MessagesComponent } from './messages.component';
import { MockDataService } from '../../core/services/mock-data.service';

describe('MessagesComponent', () => {
  let component: MessagesComponent;
  let fixture: ComponentFixture<MessagesComponent>;
  let mockDataServiceSpy: jasmine.SpyObj<MockDataService>;

  beforeEach(async () => {
    mockDataServiceSpy = jasmine.createSpyObj<MockDataService>('MockDataService', [
      'getMessagesData'
    ]);
    mockDataServiceSpy.getMessagesData.and.returnValue(
      of([
        {
          id: 'c1',
          participantId: 'p1',
          participantName: 'Dr. Test',
          participantRole: 'Doctor',
          participantAvatar: '',
          isOnline: true,
          lastMessage: 'Hello',
          lastMessageTime: 'Now',
          unreadCount: 0,
          messages: []
        }
      ])
    );

    await TestBed.configureTestingModule({
      imports: [MessagesComponent],
      providers: [
        { provide: MockDataService, useValue: mockDataServiceSpy }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(MessagesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
