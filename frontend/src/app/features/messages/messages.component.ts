import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MockDataService } from '../../core/services/mock-data.service';

interface Message {
  id: string;
  senderId: string;
  text: string;
  timestamp: string;
  isRead: boolean;
  attachment?: {
    name: string;
    size: string;
    type: string;
  };
}

interface Conversation {
  id: string;
  participantId: string;
  participantName: string;
  participantRole: string;
  participantAvatar: string;
  isOnline: boolean;
  lastMessage: string;
  lastMessageTime: string;
  unreadCount: number;
  messages: Message[];
}

@Component({
  selector: 'app-messages',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  private mockDataService = inject(MockDataService);

  currentUserId = 'user1'; // Patient
  activeTab = 'ALL';
  
  conversations: Conversation[] = [];
  activeConversation: Conversation | null = null;
  newMessageText: string = '';

  ngOnInit(): void {
    this.mockDataService.getMessagesData().subscribe({
      next: (data) => {
        this.conversations = data;
        if (data.length > 0) {
          this.activeConversation = data[0];
        }
      }
    });
  }

  selectConversation(conv: Conversation) {
    this.activeConversation = conv;
    // Mark as read
    conv.unreadCount = 0;
  }

  setTab(tab: string) {
    this.activeTab = tab;
  }

  sendMessage() {
    if (!this.newMessageText.trim() || !this.activeConversation) return;

    const newMsg: Message = {
      id: 'm_' + Date.now(),
      senderId: this.currentUserId,
      text: this.newMessageText,
      timestamp: new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' }),
      isRead: false
    };

    this.activeConversation.messages.push(newMsg);
    this.activeConversation.lastMessage = this.newMessageText;
    this.activeConversation.lastMessageTime = 'Just now';
    
    this.newMessageText = '';
    
    // Auto-scroll logic would go here in a real app
  }
}
