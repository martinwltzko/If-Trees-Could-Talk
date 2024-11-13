from rest_framework import viewsets, permissions, authentication, mixins, generics, status
from rest_framework.decorators import api_view
from rest_framework.response import Response
from rest_framework.views import APIView
from rest_framework.reverse import reverse
from service.models import AuthToken, Message
from service.serializers import MessageSerializer
from .permissions import IsOwnerOrReadOnly
from .authentications import UnityAuthentication, create_new_token


@api_view(["GET"])
def api_root(request, format=None):
    """
    API endpoint that allows players and chunks to be viewed.
    """
    return Response({
        'messages': reverse('message-list', request=request, format=format)
    })


class ObtainAuthTokenView(APIView):
    """
    API endpoint that allows a token to be obtained.
    """
    authentication_classes = [UnityAuthentication]

    def post(self, request, *args, **kwargs):
        # Perform authentication using your custom authentication class
        user, _ = UnityAuthentication().authenticate(request)

        if user is None:
            return Response({"error": "Invalid credentials"}, status=status.HTTP_401_UNAUTHORIZED)

        # Get or create the token
        token = create_new_token(user)

        # Return the token in the response
        return Response({"token": token}, status=status.HTTP_200_OK)


class MessageList(mixins.ListModelMixin, mixins.CreateModelMixin, generics.GenericAPIView):
    """
    API endpoint that allows messages to be created.
    """
    queryset = Message.objects.all()
    serializer_class = MessageSerializer
    authentication_classes = [UnityAuthentication,authentication.BasicAuthentication]
    permission_classes = [permissions.IsAuthenticatedOrReadOnly]

    def get(self, request, *args, **kwargs):
        return self.list(request, *args, **kwargs)

    def post(self, request, *args, **kwargs):
        return self.create(request, *args, **kwargs)


class MessageDetails(mixins.RetrieveModelMixin, mixins.UpdateModelMixin, generics.GenericAPIView):
    """
    API endpoint that allows messages to be viewed or edited.
    """
    queryset = Message.objects.all()
    serializer_class = MessageSerializer

    authentication_classes = [UnityAuthentication,authentication.BasicAuthentication]
    permission_classes = [permissions.IsAuthenticated,IsOwnerOrReadOnly]

    def get(self, request, *args, **kwargs):
        return self.retrieve(request, *args, **kwargs)

    def put(self, request, *args, **kwargs):
        return self.update(request, *args, **kwargs)

    def patch(self, request, *args, **kwargs):
        return self.partial_update(request, *args, **kwargs)
