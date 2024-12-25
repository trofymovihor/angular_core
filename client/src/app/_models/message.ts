export interface Message {
    id: number
    senderId: number
    senderUsername: string
    senderPhotoUrl: string
    recipientUsername: string
    recipientPhotoUrl: string
    content: string
    messageSent: Date
    messageRead?: Date
    recipientId: number
  }