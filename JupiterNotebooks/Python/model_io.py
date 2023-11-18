def read_train_data():
    train_data = []
    n, C = list(map(int, input().split()))

    if n < 1:
        raise ValueError("n must be greater than 0")
    if C < 1:
        raise ValueError("C must be greater than 0")
    
    for _ in range(n):
        data = list(map(float, input().split()))
        train_data.append(data)
    train_labels = list(map(int, input().split()))

    if len(train_data) < 1:
        raise ValueError("No training data")
    if len(train_labels) != n:
        raise ValueError("Number of labels does not match number of data points")
    
    return C, train_data, train_labels


def read_test_data():
    test_data = []
    n = int(input().strip())

    if n < 1:
        raise ValueError("n must be greater than 0")

    for _ in range(n):
        data = list(map(float, input().split()))
        test_data.append(data)

    if len(test_data) < 1:
        raise ValueError("No test data")

    return test_data


def print_train_complete():
    print(">>>")

def print_predictions(predictions):
    json = "[" + ",".join(map(str, predictions)) + "]"
    print(json)
    print(">>>")
