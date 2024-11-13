from django.core.exceptions import ValidationError
from django.core.validators import BaseValidator

class VectorFieldValidator(BaseValidator):
    def compare(self, value, limit_value):
        # Comparison logic not needed, leave as pass
        pass

    def clean(self, x):
        #Cleaning logic not needed, leave as pass
        pass

    def __call__(self, value):
        if not isinstance(value, dict):
            raise ValidationError("Value must be JSON object")
        for key in ["x", "y", "z"]:
            if key not in value or not isinstance(value[key], (int,float)):
                raise ValidationError(f"{key} must be float in the vector")