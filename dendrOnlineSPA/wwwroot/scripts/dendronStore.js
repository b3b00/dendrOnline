import { writable } from 'svelte/store';


export const repository = writable({
    id:-1,
    name:""
});

export function setRepository(repository) {
    repository.update(r => { return repository  });
}


export const repositories = writable([]);

export function setRepositories(repositories) {
    repositories.update(r => { return repositories  });
}