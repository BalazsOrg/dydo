<!-- Adapted from mattpocock/skills tdd/mocking at 6f7f7831a625f040a8e06a0e7954826b87ce775a (MIT). -->

# When to mock

Mock at **system boundaries** only:

- External services: payment, email, a hosted API.
- Databases, sometimes. Prefer a real test database.
- Time and randomness.
- The file system, sometimes. Prefer a real temporary directory.

Your own classes and modules, and every internal collaborator, run for real in the test. A mock of code you control couples the test to how the code works today, and the test breaks on a refactor that keeps the behaviour.

## Designing for mockability

At a system boundary, shape the interface so a test can replace it cheaply.

**Pass the dependency in.**

```typescript
// Easy to replace in a test
function processPayment(order, paymentClient) {
  return paymentClient.charge(order.total);
}

// Hard to replace
function processPayment(order) {
  const client = new StripeClient(process.env.STRIPE_KEY);
  return client.charge(order.total);
}
```

**Give each external operation its own function**, instead of one generic fetcher with conditional logic.

```typescript
// Each function is replaced on its own
const api = {
  getUser: (id) => fetch(`/users/${id}`),
  getOrders: (userId) => fetch(`/users/${userId}/orders`),
  createOrder: (data) => fetch('/orders', { method: 'POST', body: data }),
};

// A replacement needs conditional logic of its own
const api = {
  fetch: (endpoint, options) => fetch(endpoint, options),
};
```

Each replacement then returns one shape, the test setup holds no conditions, and the test shows which operations it exercises.
