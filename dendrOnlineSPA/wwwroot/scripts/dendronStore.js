import { writable } from 'svelte/store';


export const repository = writable({
    id:-1,
    name:""
});

export function setRepository(repo) {
    repository.update(r => { return repo });
}


export const repositories = writable([]);

export function setRepositories(repos) {
    repositories.update(r => { return repos  });
}