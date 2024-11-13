from django.contrib.auth.models import AbstractUser, User, AnonymousUser
from django.conf import settings
from django.db import models
from django.conf import settings
from django.utils import timezone
from datetime import timedelta
from .validators import VectorFieldValidator

"""
class Player(models.Model):
    user = models.OneToOneField(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    player_id = models.CharField(max_length=255, unique=True)
    created = models.DateTimeField(auto_now_add=True)

    def __str__(self):
        return self.user.username
    
    class Meta:
        ordering = ['created']
"""

#TODO
# class MessageSticker(models.Model):
#     local_x = models.FloatField(default=0.0) # Local sticker position
#     local_y = models.FloatField(default=0.0) #
#     rotation = models.FloatField(default=0.0) # Local sticker rotation on normal to message
#     date = models.DateTimeField(auto_now_add=True)


class Message(models.Model):
    owner = models.CharField(max_length=255, default='server')
    date = models.DateTimeField(auto_now_add=True)
    message = models.TextField()

    # Position and normal in world space to get persistent messages in the game world
    position = models.JSONField(default={"x":0.0,"y":0.0,"z":0.0}, validators=[VectorFieldValidator])
    normal = models.JSONField(default={"x":0.0,"y":0.0,"z":0.0}, validators=[VectorFieldValidator])

    # TODO: Message sector
    # sector = models.CharField(max_length=50, default='world')

    # TODO: Message stickers
    # stickers = models.ForeignKey(MessageSticker)


    class Meta:
        ordering = ['date']


class AuthToken(models.Model):
    user = models.OneToOneField(settings.AUTH_USER_MODEL, on_delete=models.CASCADE)
    token = models.CharField(max_length=255, unique=True)
    player_id = models.CharField(max_length=255, unique=True)
    created = models.DateTimeField(auto_now_add=True)
    expires = models.DateTimeField()

    def is_expired(self):
        return timezone.now() >= self.expires

    def renew(self):
        self.expires = timezone.now() + timedelta(hours=1)  # Example: token is valid for 1 hour
        self.save()

