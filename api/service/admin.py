from django.contrib import admin
from .models import AuthToken, Message


class AuthTokenAdmin(admin.ModelAdmin):
    list_display = ('user', 'player_id', 'token', 'created', 'expires', 'is_expired')
    search_fields = ('user_username', 'player_id', 'token')
    list_filter = ('expires',)
    #readonly_fields = ('created', 'expires')

    def is_expired(self, obj):
        return obj.is_expired()
    is_expired.boolean = True  # Display as a boolean icon in the admin interface


class MessageListAdmin(admin.ModelAdmin):
    list_display = ('owner', 'date', 'message')
    search_fields = ('owner', 'message')
    list_filter = ('date',)


admin.site.register(Message, MessageListAdmin)
admin.site.register(AuthToken, AuthTokenAdmin)

