import { Component, OnInit } from '@angular/core';
import { HubConnectionBuilder, HubConnection, IHttpConnectionOptions } from '@aspnet/signalr';
import { AuthService } from '../services/auth.service';

@Component({
	selector: 'app-chat-app',
	templateUrl: './chat-app.component.html',
	styleUrls: ['./chat-app.component.scss']
})
export class ChatAppComponent implements OnInit {
	private hubConnection: HubConnection;
	nick = '';
	message = '';
	messages: string[] = [];

	constructor(private authService: AuthService) {}
	ngOnInit() {
		this.nick = this.authService.currentUser.fullName.split(' ')[0];

		this.hubConnection = new HubConnectionBuilder().withUrl('/chat', { accessTokenFactory: () => this.authService.accessToken }).build();

		this.hubConnection
			.start()
			.then(() => console.log('Connection started!')).then(() => this.hubConnection.invoke('getPriorMessages').catch(err => console.error(err)))
			.catch(err => console.log('Error while establishing connection :('));

		this.hubConnection.on('sendToAll', (nick: string, receivedMessage: string) => {
			const text = `${nick}: ${receivedMessage}`;
			this.messages.push(text);
		});


	}

	public sendMessage(): void {
		this.hubConnection.invoke('sendToAll', this.message).catch(err => console.error(err));
	}
}
