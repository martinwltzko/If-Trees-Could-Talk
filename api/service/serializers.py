from django.utils import timezone
from django.contrib.auth.models import User
from rest_framework import serializers
from service.models import Message


class MessageSerializer(serializers.Serializer):
    id = serializers.IntegerField(read_only=True)
    owner = serializers.CharField(read_only=True)
    message = serializers.CharField()
    position = serializers.JSONField()
    normal = serializers.JSONField()

    def create(self, validated_data):
        request = self.context.get('request')
        owner = request.user if request.user.is_authenticated else 'server'
        print(f"User: {owner}")
        return Message.objects.create(
            id=validated_data.get('id'),
            position=validated_data.get('position'),
            normal=validated_data.get('normal'),
            owner=owner,
            message=validated_data.get('message'))


    def update(self, instance, validated_data):
        instance.message = validated_data.get('message', instance.message)
        instance.save()
        return instance

    class Meta:
        model = Message
        fields = ['id', 'message', 'data']