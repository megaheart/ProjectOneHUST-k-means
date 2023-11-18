# KNN model for classification

from sklearn.neighbors import KNeighborsClassifier
from sklearn.preprocessing import StandardScaler
import warnings
warnings.filterwarnings("ignore")

import model_io

C, train_data, train_labels = model_io.read_train_data()

# # Scale data
scaler = StandardScaler()
X = scaler.fit_transform(train_data)

# # Define neural network model
model = KNeighborsClassifier()

# Train model
model.fit(X, train_labels)

model_io.print_train_complete()

# Predict test data
while True:
    test_data = model_io.read_test_data()
    x_test = scaler.transform(test_data)
    predictions = model.predict(x_test)
    model_io.print_predictions(predictions)