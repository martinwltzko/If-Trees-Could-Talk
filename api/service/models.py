from django.contrib.auth.models import AbstractUser, User, AnonymousUser
from django.conf import settings
from django.db import models
from django.conf import settings
from django.utils import timezone
from datetime import timedelta

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

class Message(models.Model):
    owner = models.CharField(max_length=255, default='server')
    date = models.DateTimeField(auto_now_add=True)
    message = models.TextField()

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

