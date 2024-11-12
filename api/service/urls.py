from django.urls import path, include
from rest_framework.routers import DefaultRouter
from service.views import ObtainAuthTokenView, api_root, MessageList, MessageDetails

# The API URLs are now determined automatically by the router.
urlpatterns = [
    path('', api_root),
    path('authenticate/', ObtainAuthTokenView.as_view(), name='api_token_auth'),
    path('messages/', MessageList.as_view(), name='message-list'),
    path('messages/<str:pk>/', MessageDetails.as_view(), name='message-detail'),
]